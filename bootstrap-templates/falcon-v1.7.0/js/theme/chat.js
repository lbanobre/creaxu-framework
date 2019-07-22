/*-----------------------------------------------
|   Chat
-----------------------------------------------*/
window.utils.$document.ready(() => {

  const Event = { 
    CLICK: 'click',
    SHOWN_BS_TAB: 'shown.bs.tab',
    RESIZE: 'resize',
    KEYUP: 'keyup',
    EMOJI_BTN_CLIK: 'emojibtn.click'
  };
  const Selector = { 
    CHAT_SIDEBAR: '.chat-sidebar',
    CHAT_CONTACT: '.chat-contact',
    CHAT_CONTENT_SCROLL_AREA: '.chat-content-scroll-area',
    CHAT_CONTENT_HEADER_ACTIVE: '.card-chat-pane.active .chat-content-header',
    CHAT_CONTENT_SCROLL_AREA_ACTIVE: '.card-chat-pane.active .chat-content-scroll-area',
    CARD_CHAT_PANE_ACTIVE: '.card-chat-pane.active',
    CHAT_EMOJIAREA: '.chat-editor-area .emojiarea',
    BTN_SEND: '.btn-send',
    CHAT_FILE_UPLOAD: '.chat-file-upload',
    CARD_CHAT_CONTENT: '.card-chat-content',
    EMOJIONEAREA_EDITOR: '.emojionearea-editor',
    BTN_INFO: '.btn-info',
    CONVERSATION_INFO: '.conversation-info',
    CONTACTS_LIST_SHOW: '.contacts-list-show',
    CHAT_EDITOR_AREA: '.chat-editor-area'

  };
  const ClassName = { 
    UNREAD_MESSAGE: 'unread-message',
    TEXT_PRIMARY: 'text-primary',
    SHOW: 'show'
  };
  const DATA_KEY = {
    TARGET: 'target',
    INDEX: 'index'
  };
  const Attribute = { STYLE: 'style' };
  
  const $chatSidebar = $(Selector.CHAT_SIDEBAR);
  const $chatContact = $(Selector.CHAT_CONTACT);
  const $chatEmojiarea = $(Selector.CHAT_EMOJIAREA);
  const $chatIcons = $(Selector.BTN_SEND + ',' + Selector.CHAT_FILE_UPLOAD);
  const $btnSend = $(Selector.BTN_SEND);
  let initialEditorHeight = $(Selector.EMOJIONEAREA_EDITOR).outerHeight();
  const $chatContent = $(Selector.CARD_CHAT_CONTENT);

  // Set chat scrollbar area height
  const setChatAreaHeight = (chatContentArea, editorAreaHeight) => {
    const chatContentHeight = chatContentArea.height();
    const calculated = chatContentHeight - editorAreaHeight;
    const chatContentHeaderHeight = $(Selector.CHAT_CONTENT_HEADER_ACTIVE).outerHeight();
    const chatArea = chatContentArea.find(Selector.CHAT_CONTENT_SCROLL_AREA_ACTIVE);
    chatArea.css({ height: calculated - chatContentHeaderHeight });
  }
  
  // Set scrollbar position
  const setScrollbarPosition = ($chatArea) => {
    const scrollbar = window.Scrollbar.get($chatArea);
    scrollbar.update();
    scrollbar.scrollTo(0, scrollbar.limit.y);
  }
  
  window.utils.$document.on(Event.CLICK, Selector.CHAT_CONTACT, function(){
    const $this = $(this);

    // Hide contact list sidebar on responsive
    window.utils.$window.width() < 768 && $chatSidebar.css('left', '-100%');

    // Remove unread-message class when read
    $this.hasClass(ClassName.UNREAD_MESSAGE) && $this.removeClass(ClassName.UNREAD_MESSAGE);

  });

  $chatContact.on(Event.SHOWN_BS_TAB, function(){
    const $this = $(this);
    const $tab = $this.data(DATA_KEY.TARGET);

    $chatEmojiarea.length && $chatEmojiarea[0].emojioneArea.setText('');

    const editorHeight = $(Selector.EMOJIONEAREA_EDITOR).outerHeight();
    
    setChatAreaHeight($chatContent, editorHeight);
    $chatIcons.removeAttr(Attribute.STYLE);
    $btnSend.removeClass(ClassName.TEXT_PRIMARY);
    
    // Set scrollbar position on bottom
    const $chatArea = document.querySelector(`${$tab} ${Selector.CHAT_CONTENT_SCROLL_AREA}`);
    setScrollbarPosition($chatArea);
    
  });


  // Detect keyup event on EmojioneArea Editor
  if($chatEmojiarea.length){

    $chatEmojiarea[0].emojioneArea.on(`${Event.KEYUP} ${Event.EMOJI_BTN_CLIK}`, ($editor) => {
      const textLength = $editor.text().trim().length;
      const {offsetWidth, clientWidth} = $editor[0];
      const currentEditorHeight = $editor.outerHeight();
      const emojiLength  = $editor.find('img').length
      
      // Change color of submit button on keyup
      textLength || emojiLength ?
        $btnSend.addClass(ClassName.TEXT_PRIMARY) : 
        $btnSend.removeClass(ClassName.TEXT_PRIMARY);

      if (currentEditorHeight !== initialEditorHeight) {
        setChatAreaHeight($chatContent, currentEditorHeight);
        // Set scrollbar position on bottom
        const tabContentId = $chatContent.find(Selector.CARD_CHAT_PANE_ACTIVE).attr('id');
        const $chatArea = document.querySelector(`#${tabContentId} ${Selector.CHAT_CONTENT_SCROLL_AREA}`);
        setScrollbarPosition($chatArea);
      }

      // Align file upload and send icons when editor overflow scroll
      $chatIcons.css({ marginRight: offsetWidth === clientWidth ? 0 : '1rem' });
      initialEditorHeight = currentEditorHeight;

    });
  }

  // Open conversation info sidebar
  window.utils.$document.on(Event.CLICK, Selector.BTN_INFO,  function(){
    const $this = $(this);
    const dataIndex = $this.data(DATA_KEY.INDEX);
    const $info = $(`${Selector.CONVERSATION_INFO}[data-${DATA_KEY.INDEX}='${dataIndex}']`);
    $info.toggleClass(ClassName.SHOW);
  });

  // Show contact list sidebar on responsive
  window.utils.$document.on(Event.CLICK, Selector.CONTACTS_LIST_SHOW, () => $chatSidebar.css('left', 0));

  // Set scrollbar area height on resize
  window.utils.$window.on(Event.RESIZE, () => {

    const editorCurrentHeight = $(Selector.EMOJIONEAREA_EDITOR).outerHeight();
    setChatAreaHeight($chatContent, editorCurrentHeight);
    const chatArea = document.querySelector(Selector.CHAT_CONTENT_SCROLL_AREA_ACTIVE);
    setScrollbarPosition(chatArea);

  });
});