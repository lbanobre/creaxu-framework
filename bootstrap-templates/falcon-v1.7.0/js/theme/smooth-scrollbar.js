/*-----------------------------------------------
|   Smooth Scrollbar
-----------------------------------------------*/
window.utils.$document.ready(() => {

  const scrollbars = document.querySelectorAll('[data-scrollbar]');
  if(scrollbars.length){
    $.each(scrollbars, (item, value) => {
      const $this = $(value);
      const { from } = $this.data('scrollbar');

      window.Scrollbar.init(value);
      
      if(from === 'bottom'){
        const scrollContent = window.Scrollbar.get(value);
        scrollContent.setPosition(0, scrollContent.limit.y);
      }
    });
  }
});
