/*-----------------------------------------------
|   Emoji Picker
-----------------------------------------------*/
window.utils.$document.ready(() => {

  const Event = { 
    FOCUS: 'focus',
  };
  const Selector = { 
    EMOJIAREA: '.emojiarea',
  };
  const DATA_KEY = {
    OPTIONS: 'options',
  };

  const emojioneareas = $(Selector.EMOJIAREA);

  // Place the Blinking Text Cursor at the end of the editor text
  const placeCaretAtEnd = (el) => {

    if (!!window.getSelection && !!document.createRange) {
      const range = document.createRange();
      range.selectNodeContents(el);
      range.collapse(false);
      const sel = window.getSelection();
      sel.removeAllRanges();
      sel.addRange(range);
    } else if (!!document.body.createTextRange) {
      const textRange = document.body.createTextRange();
      textRange.moveToElementText(el);
      textRange.collapse(false);
      textRange.select();
    }
  };


  // EmojioneArea plugin call
  if (emojioneareas.length) {
    emojioneareas.each((item, value) => {
      const $this = $(value);
      const options = $.extend({}, $this.data(DATA_KEY.OPTIONS));
      $this.emojioneArea(options);

      // // Call the caret position function on focus
      emojioneareas[item].emojioneArea.on(Event.FOCUS, ($editor) => {
        placeCaretAtEnd($editor.get(0));
      });
    });
  }
});
