window.PlayAudioFile = function (src) {
	var aplayerName = src.replaceAll('/', '_').replaceAll('.', '_');
	var audio = document.getElementById(aplayerName);
	if (audio === undefined || audio === null) {
		var audio = document.createElement('audio');
		audio.id = aplayerName;
		audio.src = src;
		audio.type = 'audio/mpeg';
		document.body.appendChild(audio);
	}
	console.log("Audio Stats");
	console.log(audio.paused);
	console.log(audio.currentTime);
	console.log(audio.ended);
	if (!audio.paused && audio.currentTime > 0 && !audio.ended) {
		console.log("Reset current Time");
		audio.currentTime = 0;
	}
	if (audio.paused && audio.currentTime > 0) {
		console.log("Reset current Time because it is paused");
		audio.currentTime = 0;
		audio.play();
	}
	else {
		audio.play();
	}

}
window.StopAllAudio = function () {
	var element = document.getElementsByTagName("audio");
	for (index = element.length - 1; index >= 0; index--) {
		if (element.id !== 'audioPlayer') {
			var audio = element[index];
			if (!audio.paused && audio.currentTime > 0 && !audio.ended) {
				audio.pause();
			}
		}

	}

}
window.ClearAudio = function () {
	var element = document.getElementsByTagName("audio");
	let index = 0;

	for (index = element.length - 1; index >= 0; index--) {
		if (element.id !== 'audioPlayer') {
			element[index].parentNode.removeChild(element[index]);
		}

	}

}