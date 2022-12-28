using BapButton;
using BAP.Types;
using BAP.Helpers;

namespace BAP.Web.Games
{

    public enum MarqueType
    {
        MultiLineScrollStaggered = 1,
        MutliLineScrollEvenSpacing = 2,
        MultiLineNoAdjustment = 3,
        AllButtonsSnakeScroll = 4
    }
    public class TextMarqueGame : MarqueGameBase
    {
        IGameHandler GameHandler { get; set; }
        ILayoutProvider LayoutProvider { get; set; }
        public bool Repeat { get; set; } = true;
        public List<MarqueType> MarqueTypes { get; set; } = new();
        private int CurrentMarqueTypeLocation { get; set; } = 0;
        public TextMarqueGame(ILayoutProvider layoutProvider, IGameHandler gameHandler, ILogger<TextMarqueGame> logger, IBapMessageSender messageSender, AnimationController animationController, ISubscriber<AnimationCompleteMessage> animationCompletePipe) : base(logger, messageSender, animationController, animationCompletePipe)
        {
            GameHandler = gameHandler;
            LayoutProvider = layoutProvider;
        }

        public async override Task<bool> Start()
        {
            base.AnimationRate = 125;
            if (MarqueTypes.Count == 0)
            {
                MarqueTypes = new List<MarqueType>() { MarqueType.MultiLineScrollStaggered, MarqueType.AllButtonsSnakeScroll, MarqueType.MutliLineScrollEvenSpacing, MarqueType.MultiLineNoAdjustment };
            }
            if (LayoutProvider == null || LayoutProvider.CurrentButtonLayout == null)
            {
                MsgSender.SendUpdate($"Cannot Start - Without a layout this is just a garbled mess.", fatalError: true);
                return false;
            }
            BapColor bapColor = StandardColorPalettes.Default[2];
            string textToDisplay = "Happy Birthday Mads!!";
            MarqueType marqueType = MarqueTypes[CurrentMarqueTypeLocation];
            if (marqueType == MarqueType.AllButtonsSnakeScroll)
            {

                List<ulong[,]> images = new();

                foreach (char letter in textToDisplay.ToCharArray())
                {
                    if (char.IsWhiteSpace(letter))
                    {
                        images.Add(new ulong[8, 8]);
                    }
                    else
                    {
                        images.Add(AnimationHelper.GetMatrix(EnumHelper.GetEnumFromCharacter(letter), bapColor));
                    }
                }

                Lines.Add(new MarqueLine() { Images = images, NodeIdsOrderedLeftToRight = MsgSender.GetConnectedButtonsInOrder() });

            }
            else
            {
                List<string> textToDisplayList = textToDisplay.Split(' ').ToList();
                if (LayoutProvider == null || LayoutProvider.CurrentButtonLayout == null || LayoutProvider.CurrentButtonLayout?.RowCount < 3)
                {
                    MsgSender.SendUpdate($"You need the same amount of words as Rows. You have {textToDisplayList.Count} words but only {(LayoutProvider?.CurrentButtonLayout?.RowCount ?? 0)} rows", fatalError: true);
                    return false;
                }
                int maxWordLength = textToDisplayList.Select(t => t.Length).Max();
                switch (marqueType)
                {
                    case MarqueType.MultiLineScrollStaggered:
                        int multiMaxWordLength = textToDisplayList.Select(t => t.Length).Max() + (2 * (textToDisplayList.Count - 1));
                        for (int i = 1; i < textToDisplayList.Count; i++)
                        {
                            textToDisplayList[i] = textToDisplayList[i].PadLeft(maxWordLength + (2 * i));
                        }
                        for (int i = 0; i < textToDisplayList.Count; i++)
                        {
                            textToDisplayList[i] = textToDisplayList[i].PadRight(multiMaxWordLength);
                        }
                        break;
                    case MarqueType.MutliLineScrollEvenSpacing:

                        textToDisplayList.ForEach(t => t = t.PadBoth(maxWordLength));
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < textToDisplayList.Count; i++)
                {
                    List<ulong[,]> images = new();

                    foreach (char letter in textToDisplayList[i].ToCharArray())
                    {
                        if (char.IsWhiteSpace(letter))
                        {
                            images.Add(new ulong[8, 8]);
                        }
                        else
                        {
                            images.Add(AnimationHelper.GetMatrix(EnumHelper.GetEnumFromCharacter(letter), bapColor));
                        }
                    }

                    Lines.Add(new MarqueLine() { Images = images, NodeIdsOrderedLeftToRight = LayoutProvider.CurrentButtonLayout.ButtonPositions.Where(t => t.RowId == i + 1).OrderBy(t => t.ColumnId).Select(t => t.ButtonId).ToList() });
                }

            }


            ScrollAllTextOffScreen = true;
            return await base.Start();
        }

        public override async Task<bool> AnimationComplete(AnimationCompleteMessage animationCompleteMessage)
        {
            if (Repeat)
            {
                if (CurrentMarqueTypeLocation == MarqueTypes.Count - 1)
                {
                    CurrentMarqueTypeLocation = 0;
                }
                else
                {
                    CurrentMarqueTypeLocation++;
                }
                return await Start();
            }
            else
            {
                End("Marquee Completed");
                return true;
            }
        }
    }
}
