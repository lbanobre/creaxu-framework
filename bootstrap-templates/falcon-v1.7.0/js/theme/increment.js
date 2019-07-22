'use strict';

import utils from './Utils';

/*-----------------------------------------------
|   Tabs
-----------------------------------------------*/
utils.$document.ready(() => {

  $(document).on('click', '[data-field]', (e) => {
    const $this = $(e.target);
    const inputField = $this.data('field');
    const $inputField = $this.parents('.input-group').children('.' + inputField);
    const $btnType = $this.data('type');
    let value = parseInt($inputField.val(), 10);
    let min = $inputField.data('min');

    if(min){
      min = parseInt(min, 10);
    }else{
      min = 0;
    }


    if($btnType === 'plus'){
      value += 1
    }else{
      if(value > min)
        value -= 1
    }
    $inputField.val(value);


  });

});
