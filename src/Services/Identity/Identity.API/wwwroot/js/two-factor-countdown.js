import {CountDownTimer} from './CountDownTimer.js';

window.onload = function() {

    document.getElementById('twofactor-options').style.marginTop = "10px"
    
    let display = document.querySelector('#code-timer');
    let now = new Date();
    let unixAllowedAt = new Date(Date.parse(document.getElementById('allowed-to-resend-at').textContent));
    let currentDate = new Date(now.getUTCFullYear(),now.getUTCMonth(), now.getUTCDate() ,
        now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds(), now.getUTCMilliseconds());
    let timeDiff = unixAllowedAt.getTime() - currentDate.getTime(); // in miliseconds
    let secondsUntilAllowed = Math.round(timeDiff / 1000) // in seconds
    
    if (display){
        let timer = new CountDownTimer(secondsUntilAllowed);
        timer.onTick(format).start();
        setTimeout(enableCodeRequest, secondsUntilAllowed * 1000);
    }
    
    function format(minutes, seconds) {
        minutes = minutes > 0 ?  minutes + (minutes > 1 ? ' minutes and' : ' minute and') : '';
        seconds = seconds + (seconds > 1 ? ' seconds' : ' second');
        display.textContent = minutes + ' ' + seconds;
    }

    function enableCodeRequest(){
        document.getElementById('timer-button').style.display = "inline"
        document.getElementById('timer-text').style.display = "none"
    }
};
