/*-----------------------------------------------
|   Toastr
-----------------------------------------------*/
window.utils.$document.ready(() => {
  const ratings = $(".raty");

  if(ratings.length){
    ratings.each((index, value) => {
      const $this = $(value);
      const options = $.extend({
          starHalf: 'star-half-png text-warning',
          starOff: 'star-off-png text-300',
          starOn: 'star-on-png text-warning',
          starType: 'span',
          size: 30
        },$this.data('options'));
      
      $(value).raty(options);
    });
  }
});
