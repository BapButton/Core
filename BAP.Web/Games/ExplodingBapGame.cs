using System.IO;
using System.Threading;
using BAP.Web.Pages.Games;

namespace BAP.Web.Games
{


    internal class OverlappingImage
    {
        internal int ImageId { get; set; }
        internal List<OverlappingImageNode> OverlappingImageNodes { get; set; } = new();
    }
    internal class OverlappingImageNode
    {
        internal string NodeId { get; set; }
        internal int InitialPositionFromLeft { get; set; }
        internal int NumberOfPixelColumnsShowing { get; set; }
    }

    internal class ExplosionTracker
    {
        internal List<string> InitialNodeIds { get; set; } = new();
        internal List<string> ActiveNodeIds { get; set; } = new();
        internal int FrameIdOfCurrentExplosion { get; set; }
    }

    internal class ExplodingBapRow
    {
        internal bool ReversedDirection { get; set; }
        internal int[] ImageTrackingArray { get; set; } = default!;
        internal List<string> NodeIdsOrderedLeftToRight { get; set; } = new();

    }
    public class ExplodingBapGame : IBapGame
    {

        public bool IsGameRunning { get; set; }
        ILogger<ExplodingBapGame> Logger { get; set; }
        IBapMessageSender MsgSender { get; set; } = default!;
        List<string> NodeIdsPressed { get; set; } = new();
        public int FrameSpacing { get; set; } = 0;
        List<ulong[,]> PossibleImages { get; set; } = new();
        IGameProvider GameHandler { get; set; } = default!;
        ILayoutProvider LayoutProvider { get; set; } = default!;
        bool ExplosionInProcess;
        ExplosionTracker? currentExplosionTracker = null;
        CancellationTokenSource timerTokenSource = new();
        IDisposable subscriptions = default!;
        List<ExplodingBapRow> Rows { get; set; } = new();
        List<string> firstAndLastColumnNodeIds = new();
        int FrameTrackingCount = 0;
        ISubscriber<ButtonPressedMessage> ButtonPressedPipe { get; set; } = default!;
        public int SpeedMultiplier = 8;
        PeriodicTimer? timer = null;

        //int minOverlap = 1;
        readonly int failOverlap = 8;
        public ExplodingBapGame(ILogger<ExplodingBapGame> logger, IBapMessageSender messageSender, IGameProvider gameHandler, ILayoutProvider layoutProvider, ISubscriber<ButtonPressedMessage> buttonPressedPipe)
        {
            Logger = logger;
            MsgSender = messageSender;
            GameHandler = gameHandler;
            ButtonPressedPipe = buttonPressedPipe;
            var bag = DisposableBag.CreateBuilder();
            ButtonPressedPipe.Subscribe((x) => ButtonPressed(x)).AddTo(bag);
            subscriptions = bag.Build();
        }

        public async Task<bool> Start()
        {
            IsGameRunning = true;
            ExplosionInProcess = false;
            Rows = new List<ExplodingBapRow>();
            if (PossibleImages.Count == 0)
            {
                string path = Path.Combine(".", "wwwroot", "sprites", "ExplodingBap.bmp");
                PossibleImages = new SpriteParser(path).GetCustomMatricesFromCustomSprite();
            }
            if (LayoutProvider == null || LayoutProvider?.CurrentButtonLayout == null)
            {
                MsgSender.SendUpdate("Exploding Bap requires a button Layout", fatalError: true);
                await ForceEndGame();
                return false;
            }
            foreach (var row in LayoutProvider.CurrentButtonLayout.ButtonPositions.GroupBy(t => t.RowId).OrderBy(t => t.Key))
            {
                firstAndLastColumnNodeIds.Add(row.OrderBy(t => t.ColumnId).First().ButtonId);
                firstAndLastColumnNodeIds.Add(row.OrderByDescending(t => t.ColumnId).First().ButtonId);
                int arrayWidth = (row.Count() * 8 + FrameSpacing) - FrameSpacing;
                var newRow = new ExplodingBapRow()
                {
                    ReversedDirection = row.Key % 2 == 0,
                    ImageTrackingArray = new int[arrayWidth],
                    NodeIdsOrderedLeftToRight = row.Select(t => t.ButtonId).ToList()
                };
                for (int i = 0; i < arrayWidth; i++)
                {
                    newRow.ImageTrackingArray[i] = -1;
                }
                Rows.Add(newRow); ;
            }

            MoveToNextFrame();
            //As we are not awaiting this task. It will just keep running until the cancellation token fires.
            Task TimerTask = StartGameFrameTicker();

            return true;
        }

        private async Task StartGameFrameTicker()
        {
            if (timerTokenSource.IsCancellationRequested)
            {
                timerTokenSource = new();

            }
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));
            var timerToken = timerTokenSource.Token;

            while (await timer.WaitForNextTickAsync(timerToken))
            {
                try
                {
                    MoveToNextFrame();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in Move To Next Frame Method");
                }
            };
            Logger.LogError("The Timer Has Stopped");
        }

        private void ShiftEverythingAndShowIt()
        {
            List<(string nodeId, ButtonImage image)> images = new();
            foreach (var row in Rows)
            {
                int fullFrameLength = row.ImageTrackingArray.Length;
                int lastItemInArray = fullFrameLength - 1;
                //Regular direction left to right
                if (!row.ReversedDirection)
                {
                    //Shift it all
                    for (int i = 0; i < fullFrameLength - 1; i++)
                    {
                        row.ImageTrackingArray[i] = row.ImageTrackingArray[i + 1];
                    }
                    int currentImage = -1;
                    //There used to be an image here;
                    if (row.ImageTrackingArray[lastItemInArray] > -1)
                    {
                        currentImage = row.ImageTrackingArray[lastItemInArray];
                        int endOfImage = lastItemInArray;
                        for (int i = lastItemInArray; i >= lastItemInArray - 8; i--)
                        {
                            if (row.ImageTrackingArray[i] != currentImage)
                            {
                                endOfImage = i;
                                break;
                            }
                        }
                        //If the image is already taking up 8 columns then we need to put in a blank column;
                        if (endOfImage == lastItemInArray)
                        {
                            if (FrameSpacing > 0)
                            {
                                row.ImageTrackingArray[lastItemInArray] = -1;
                            }
                            else
                            {
                                row.ImageTrackingArray[lastItemInArray] = BapBasicGameHelper.GetRandomInt(0, PossibleImages.Count, currentImage, 50);

                            }
                        }
                    }
                    //If there was already a blank column then if there is now the framespacing of blank columns then a new random image is needed.
                    else
                    {
                        int endOfBlankSpace = 0;
                        for (int i = lastItemInArray; i > (lastItemInArray - FrameSpacing); i--)
                        {
                            if (row.ImageTrackingArray[i] > currentImage)
                            {
                                endOfBlankSpace = i;
                            }
                        }
                        if (endOfBlankSpace == 0 || FrameSpacing == 0)
                        {
                            row.ImageTrackingArray[lastItemInArray] = BapBasicGameHelper.GetRandomInt(0, PossibleImages.Count, currentImage, 50);
                        }
                        else
                        {
                            row.ImageTrackingArray[lastItemInArray] = -1;
                        }
                    }
                }
                //reversed Direction Right to left 
                else
                {
                    for (int i = lastItemInArray; i > 0; i--)
                    {
                        row.ImageTrackingArray[i] = row.ImageTrackingArray[i - 1];
                    }
                    int currentImage = -1;
                    //There used to be an image here;
                    if (row.ImageTrackingArray[0] > -1)
                    {
                        currentImage = row.ImageTrackingArray[0];
                        int endOfImage = 0;
                        for (int i = 0; i <= 8; i++)
                        {
                            if (row.ImageTrackingArray[i] != currentImage)
                            {
                                endOfImage = i;
                                break;
                            }
                        }
                        //If the image is already taking up 8 columns then we need to put in a blank column;
                        if (endOfImage == 0)
                        {
                            if (FrameSpacing > 0)
                            {
                                row.ImageTrackingArray[0] = -1;
                            }
                            else
                            {
                                row.ImageTrackingArray[0] = BapBasicGameHelper.GetRandomInt(0, PossibleImages.Count, currentImage, 50);

                            }

                        }
                    }
                    //If there was already a blank column and there is now the framespacing of blank columns then a new random image is needed.
                    else
                    {
                        int endOfBlankSpace = FrameSpacing;
                        for (int i = 0; i < FrameSpacing; i++)
                        {
                            if (row.ImageTrackingArray[i] > -1)
                            {
                                endOfBlankSpace = i;
                                break;
                            }
                        }
                        if (endOfBlankSpace == FrameSpacing)
                        {
                            row.ImageTrackingArray[0] = BapBasicGameHelper.GetRandomInt(0, PossibleImages.Count, currentImage, 50);
                        }
                        else
                        {
                            //More spacing is neeeded
                            row.ImageTrackingArray[0] = -1;
                        }
                    }
                }

                images.AddRange(GenerateImagesForRow(row));

            }

            foreach (var image in images)
            {
                MsgSender.SendImage(image.nodeId, image.image);
            }
        }

        private List<(string NodeId, ButtonImage buttonImage)> GenerateImagesForRow(ExplodingBapRow row)
        {
            int fullFrameLength = row.ImageTrackingArray.Length;
            List<(string nodeId, ButtonImage buttonImage)> images = new();
            ulong[,] bigMatrix = new ulong[8, fullFrameLength];

            for (int i = 0; i < row.ImageTrackingArray.Length;)
            {

                if (row.ImageTrackingArray[i] > -1)
                {
                    int imageId = row.ImageTrackingArray[i];
                    ulong[,] image = PossibleImages[imageId];
                    if (i == 0)
                    {
                        //Check if it is a partial image and then show as much of it as needed. Then move i forward.
                        int startingPosition = 0;
                        for (int j = 0; j < 8; j++)
                        {
                            if (row.ImageTrackingArray[j] != imageId)
                            {
                                startingPosition = j - 8;
                                i = j;
                                break;
                            }
                        }
                        if (startingPosition == 0)
                        {
                            i = 8;
                        }
                        if (i == -1)
                        {
                            i = 0;
                        }
                        if (i == 0)
                        {
                            i = 1;
                        }
                        AnimationHelper.MergeMatrices(bigMatrix, image, 0, startingPosition, false);
                    }
                    else
                    {
                        //The Merge Matricies will drop any portion of the override matrix that does not fit in the frame. 
                        AnimationHelper.MergeMatrices(bigMatrix, image, 0, i, false);
                        //If this is greater the I then we have reached the end
                        i += 8;
                    }
                }
                else
                {
                    i++;
                }
            }
            //Now that the big matrix is made we can cut it up into frames;
            for (int i = 0; i < row.NodeIdsOrderedLeftToRight.Count; i++)
            {
                images.Add((row.NodeIdsOrderedLeftToRight[i], new ButtonImage(bigMatrix.ExtractMatrix(0, (i * 8)))));
            }
            return images;
        }

        private void MoveToNextFrame()
        {
            if (!ExplosionInProcess)
            {
                var overlappingImages = GetCurrentlyOverlappingNodes();
                EvaluateButtonPresses(overlappingImages);
                EvaluateIfTheGameHasEnded(overlappingImages);
            }

            if (ExplosionInProcess)
            {

                if (FrameTrackingCount % 4 == 0)
                {
                    ShowTheExplosion();
                }

            }
            else
            {
                if (FrameTrackingCount % SpeedMultiplier == 0)
                {
                    ShiftEverythingAndShowIt();
                }

            }
            FrameTrackingCount++;

        }
        private void ButtonPressed(ButtonPressedMessage buttonPressedMessage)
        {
            NodeIdsPressed.Add(buttonPressedMessage.NodeId);
        }

        private List<OverlappingImage> GetCurrentlyOverlappingNodes()
        {
            List<OverlappingImage> overlappingImages = new();
            int longestRow = Rows.Select(t => t.ImageTrackingArray.Length).Max();
            int mostButtons = longestRow / 8;
            for (int i = 0; i < mostButtons; i++)
            {
                List<OverlappingImage>? currentlyShowingImagesOnAllButtonsInThisColumn = null;
                foreach (var row in Rows)
                {
                    Dictionary<int, (int pixelsShowing, int startingPosition)> currentlyShowingImagesOnThisButton = new();
                    for (int j = i * 8; j < (i * 8) + 8; j++)
                    {
                        int currentValue = row.ImageTrackingArray[j];
                        if (currentValue > -1)
                        {
                            if (currentlyShowingImagesOnThisButton.ContainsKey(currentValue))
                            {
                                var currentItem = currentlyShowingImagesOnThisButton[currentValue];
                                currentlyShowingImagesOnThisButton[currentValue] = (currentItem.pixelsShowing + 1, currentItem.startingPosition);
                            }
                            else
                            {
                                currentlyShowingImagesOnThisButton.Add(currentValue, (1, j));
                            }
                        }

                    }
                    if (currentlyShowingImagesOnAllButtonsInThisColumn == null)
                    {
                        currentlyShowingImagesOnAllButtonsInThisColumn = new();
                        foreach (var item in currentlyShowingImagesOnThisButton)
                        {
                            OverlappingImageNode node = new() { NodeId = row.NodeIdsOrderedLeftToRight[i], InitialPositionFromLeft = item.Value.startingPosition, NumberOfPixelColumnsShowing = item.Value.pixelsShowing };
                            currentlyShowingImagesOnAllButtonsInThisColumn.Add(new OverlappingImage() { ImageId = item.Key, OverlappingImageNodes = new() { node } });
                        }
                    }
                    else
                    {
                        foreach (var item in currentlyShowingImagesOnThisButton)
                        {
                            var currentItem = currentlyShowingImagesOnAllButtonsInThisColumn.FirstOrDefault(t => t.ImageId == item.Key);
                            if (currentItem != null)
                            {
                                currentItem.OverlappingImageNodes.Add(new() { NodeId = row.NodeIdsOrderedLeftToRight[i], NumberOfPixelColumnsShowing = item.Value.pixelsShowing, InitialPositionFromLeft = item.Value.startingPosition });
                            }
                        }
                        List<int> imageIdsShowingThisRound = currentlyShowingImagesOnThisButton.Select(t => t.Key).ToList();
                        //Drop anything not found on this Loop;
                        currentlyShowingImagesOnAllButtonsInThisColumn = currentlyShowingImagesOnAllButtonsInThisColumn.Where(t => imageIdsShowingThisRound.Contains(t.ImageId)).ToList();
                    }
                    if (currentlyShowingImagesOnAllButtonsInThisColumn.Count == 0)
                    {
                        break;
                    }
                }
                if (currentlyShowingImagesOnAllButtonsInThisColumn?.Count > 0)
                {
                    overlappingImages.AddRange(currentlyShowingImagesOnAllButtonsInThisColumn);
                }
            }


            return overlappingImages;
        }

        private void ShowTheExplosion()
        {
            List<(string nodeId, ButtonImage buttonImage)> allCurrentImages = new();
            foreach (var row in Rows)
            {
                allCurrentImages.AddRange(GenerateImagesForRow(row));
            }
            if (currentExplosionTracker != null)
            {
                bool endTheGame = false;
                foreach (var nodeId in currentExplosionTracker.ActiveNodeIds)
                {
                    var currentMessage = allCurrentImages.FirstOrDefault(t => t.nodeId == nodeId);
                    if (currentMessage != default)
                    {
                        ulong[,] explosionOverLay = new ulong[8, 8];
                        int overLayRow = 0;
                        int overLayColumn = 0;

                        ulong white = new BapColor(255, 255, 255).LongColor;
                        if (currentExplosionTracker.FrameIdOfCurrentExplosion >= 0)
                        {
                            ulong[,] tempOverlay = new ulong[2, 2];
                            tempOverlay[0, 0] = white;
                            tempOverlay[0, 1] = white;
                            tempOverlay[1, 0] = white;
                            tempOverlay[1, 1] = white;
                            explosionOverLay.MergeMatrices(tempOverlay, 3, 3);

                        }
                        if (currentExplosionTracker.FrameIdOfCurrentExplosion >= 3)
                        {
                            ulong[,] tempOverlay = new ulong[4, 4];
                            tempOverlay[0, 0] = white;
                            tempOverlay[0, 3] = white;
                            tempOverlay[3, 0] = white;
                            tempOverlay[0, 3] = white;
                            explosionOverLay.MergeMatrices(tempOverlay, 2, 2);
                        }
                        if (currentExplosionTracker.FrameIdOfCurrentExplosion >= 2)
                        {
                            ulong[,] tempOverlay = new ulong[6, 6];
                            tempOverlay[0, 0] = white;
                            tempOverlay[0, 5] = white;
                            tempOverlay[1, 4] = white;
                            tempOverlay[3, 5] = white;
                            tempOverlay[5, 2] = white;
                            tempOverlay[5, 4] = white;
                            explosionOverLay.MergeMatrices(tempOverlay, 1, 1);
                        }
                        if (currentExplosionTracker.FrameIdOfCurrentExplosion >= 3)
                        {
                            ulong[,] tempOverlay = new ulong[8, 8];
                            for (int i = 0; i < (8 * currentExplosionTracker.FrameIdOfCurrentExplosion); i++)
                            {
                                tempOverlay[Random.Shared.Next(0, 8), Random.Shared.Next(0, 8)] = white;
                            }
                            explosionOverLay.MergeMatrices(tempOverlay, 0, 0);
                        }
                        if (currentExplosionTracker.FrameIdOfCurrentExplosion >= 10)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    explosionOverLay[i, j] = white;
                                }
                            }
                            endTheGame = true;
                        }
                        currentMessage.buttonImage.GetImageMatrix().MergeMatrices(explosionOverLay, overLayRow, overLayColumn);
                        MsgSender.SendImage(currentMessage.nodeId, currentMessage.buttonImage);

                    }

                }
                if (endTheGame)
                {
                    ForceEndGame();
                }
                currentExplosionTracker.FrameIdOfCurrentExplosion++;
            }

        }


        private void EvaluateIfTheGameHasEnded(List<OverlappingImage> overlappingImages)
        {
            foreach (var overlappingImage in overlappingImages)
            {

                if (overlappingImage.OverlappingImageNodes.Where(t => t.NumberOfPixelColumnsShowing >= failOverlap).Count() == overlappingImage.OverlappingImageNodes.Count())
                {
                    if (overlappingImage.OverlappingImageNodes.Select(t => t.NodeId).Intersect(firstAndLastColumnNodeIds).Count() == 0)
                    {
                        ExplosionInProcess = true;
                        var nodes = overlappingImage.OverlappingImageNodes.Select(t => t.NodeId).ToList();
                        currentExplosionTracker = new ExplosionTracker()
                        {
                            ActiveNodeIds = nodes,
                            InitialNodeIds = nodes
                        };

                    }
                }

            }

        }

        private void EvaluateButtonPresses(List<OverlappingImage> overlappingImages)
        {
            if (NodeIdsPressed.Count > 0)
            {
                List<string> nodeIdsToRemove = new List<string>();
                foreach (var nodeId in NodeIdsPressed)
                {
                    if (!overlappingImages.SelectMany(t => t.OverlappingImageNodes).Select(t => t.NodeId).Contains(nodeId))
                    {
                        nodeIdsToRemove.Add(nodeId);
                    }
                }
                foreach (var nodeId in nodeIdsToRemove)
                {
                    NodeIdsPressed.Remove(nodeId);
                }
                List<OverlappingImage> completedOverlappingNodes = new();
                foreach (var overlap in overlappingImages)
                {
                    List<string> allRequiredNodeIds = overlap.OverlappingImageNodes.Select(t => t.NodeId).ToList();
                    if (NodeIdsPressed.Intersect(allRequiredNodeIds).Count() == allRequiredNodeIds.Count)
                    {
                        completedOverlappingNodes.Add(overlap);
                    }

                }

                if (completedOverlappingNodes.Count > 0)
                {
                    foreach (var completedImage in completedOverlappingNodes)
                    {
                        foreach (var nodeImage in completedImage.OverlappingImageNodes)
                        {
                            var currentRow = Rows.FirstOrDefault(t => t.NodeIdsOrderedLeftToRight.Contains(nodeImage.NodeId));
                            if (currentRow != null)
                            {
                                int initialPosition = nodeImage.InitialPositionFromLeft;
                                if ((nodeImage.InitialPositionFromLeft) % 8 == 0 || nodeImage.InitialPositionFromLeft == 0)
                                {
                                    //this means that it is covering the left side of the button.
                                    initialPosition = nodeImage.InitialPositionFromLeft + nodeImage.NumberOfPixelColumnsShowing - 8;

                                }
                                initialPosition = initialPosition < 0 ? 0 : initialPosition;
                                for (int i = initialPosition; i < Math.Min(initialPosition + 8, currentRow.ImageTrackingArray.Length); i++)
                                {
                                    currentRow.ImageTrackingArray[i] = -1;
                                }
                            }

                        }
                        overlappingImages.Remove(completedImage);
                    }

                }
            }

        }





        public Task<bool> ForceEndGame()
        {
            timerTokenSource.Cancel();
            IsGameRunning = false;
            return Task.FromResult(true); ;
        }
        public void Dispose()
        {
            if (timerTokenSource != null)
            {
                timerTokenSource.Cancel();
                timerTokenSource.Dispose();
            }
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }
        }

    }
}
