'use strict';

import utils from './Utils';

/*-----------------------------------------------
|  Flex slider
-----------------------------------------------*/

utils.$document.ready(() => {
  const flexslider = $('.flexslider');
  if(flexslider.length){

    flexslider.each((item, value) => {    
      const $this = $(value);
      
      $this.flexslider($.extend({
          prevText: '<span class="indicator-arrow indicator-arrow-left"></span>',
          nextText: '<span class="indicator-arrow indicator-arrow-right"></span>',
        },
        $this.data("options")
      ));
    });
  }

});