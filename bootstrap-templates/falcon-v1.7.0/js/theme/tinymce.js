/*-----------------------------------------------
|   CKEditor | WYSIWYG
-----------------------------------------------*/
window.utils.$document.ready(() => {


  const tinymces = $(".tinymce");

  if(tinymces.length){
    window.tinymce.init({
      selector: '.tinymce',
      height: '50vh',
      menubar: false,
      mobile: {
        theme: 'mobile',
        toolbar: ['undo', 'bold'],
      },
      statusbar: false,
      plugins: 'link,image,lists,table,media',
      toolbar: 'styleselect | bold italic link bullist numlist image blockquote table media undo redo'
    });
  }

  const toggle = $('#progress-toggle-animation');
  toggle.on('click', () => $('#progress-toggle').toggleClass('progress-bar-animated'));
});
