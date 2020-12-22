$(document).ready(function () {
    let htmlElement = $('#code-wrapper')

    function goToNextInput(event) {
        let key = event.which,
            t = $(event.target),
            sib = t.next('input')

        if ($(event.target).val() === '') {
            return true
        }

        if (key !== 9 && !isDigitKey(key)) {
            event.preventDefault()
            return false
        }

        if (key === 9) {
            return true
        }

        sib.select().focus()
    }

    function onKeyDown(event) {
        let key = event.which

        if (key === 9 || isDigitKey(key.valueOf())) {
            return true
        }

        if (key === 8 || key === 46) {
            clearInput(event)
        }

        event.preventDefault()
        return false
    }

    function onFocus(event) {
        $(event.target).select()
    }

    function clearInput(event) {
        let currentInput = $(event.target)
        let value = currentInput.val()
        if (!value) {
            currentInput.prev('input').val('')
            currentInput.prev('input').select().focus()
        }
        currentInput.val('')
        currentInput.prev('input').select().focus()
        event.preventDefault()
    }

    function isDigitKey(keyCode) {
        if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105)) {
            return true
        }
        return false
    }

    htmlElement.on('keyup', 'input', goToNextInput)
    htmlElement.on('keydown', 'input', onKeyDown)
    htmlElement.on('click', 'input', onFocus)
})
