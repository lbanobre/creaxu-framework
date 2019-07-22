/*-----------------------------------------------
|   Bulk Actions
-----------------------------------------------*/
window.utils.$document.ready(() => {
  const checkboxBulkSelects = $('.checkbox-bulk-select');
  if (checkboxBulkSelects.length) {
    const Event = { CLICK: 'click' };
    const Selector = { CHECKBOX_BULK_SELECT_CHECKBOX: '.checkbox-bulk-select-target' };
    const ClassName = { D_NONE: 'd-none' };
    const DATA_KEY = {
      CHECKBOX_BODY: 'checkbox-body',
      CHECKBOX_ACTIONS: 'checkbox-actions',
      CHECKBOX_REPLACED_ELEMENT: 'checkbox-replaced-element',
    };
    const Attribute = {
      CHECKED: 'checked',
      INDETERMINATE: 'indeterminate',
    };
    checkboxBulkSelects.each((index, value) => {
      const checkboxBulkAction = $(value);
      const bulkActions = $(checkboxBulkAction.data(DATA_KEY.CHECKBOX_ACTIONS));
      const replacedElement = $(checkboxBulkAction.data(DATA_KEY.CHECKBOX_REPLACED_ELEMENT));
      
      const rowCheckboxes = $(checkboxBulkAction
        .data(DATA_KEY.CHECKBOX_BODY))
        .find(Selector.CHECKBOX_BULK_SELECT_CHECKBOX);

      checkboxBulkAction.on(Event.CLICK, () => {
        if (checkboxBulkAction.attr(Attribute.INDETERMINATE) === Attribute.INDETERMINATE) {
          bulkActions.addClass(ClassName.D_NONE);
          replacedElement.removeClass(ClassName.D_NONE);
          checkboxBulkAction
            .prop(Attribute.INDETERMINATE, false)
            .attr(Attribute.INDETERMINATE, false);
          checkboxBulkAction
            .prop(Attribute.CHECKED, false)
            .attr(Attribute.CHECKED, false);
          rowCheckboxes
            .prop(Attribute.CHECKED, false)
            .attr(Attribute.CHECKED, false);
        } else {
          bulkActions.toggleClass(ClassName.D_NONE);
          replacedElement.toggleClass(ClassName.D_NONE);
          if (checkboxBulkAction.attr(Attribute.CHECKED)) {
            checkboxBulkAction
              .prop(Attribute.CHECKED, false)
              .attr(Attribute.CHECKED, false);
          } else {
            checkboxBulkAction
              .prop(Attribute.CHECKED, true)
              .attr(Attribute.CHECKED, true);
          }
          rowCheckboxes.each((i, v) => {
            const $this = $(v);
            if ($this.attr(Attribute.CHECKED)) {
              $this
                .prop(Attribute.CHECKED, false)
                .attr(Attribute.CHECKED, false);
            } else {
              $this
                .prop(Attribute.CHECKED, true)
                .attr(Attribute.CHECKED, true);
            }
          });
        }
      });
      rowCheckboxes.on(Event.CLICK, (e) => {
        const $this = $(e.target);
        if ($this.attr(Attribute.CHECKED)) {
          $this
            .prop(Attribute.CHECKED, false)
            .attr(Attribute.CHECKED, false);
        } else {
          $this
            .prop(Attribute.CHECKED, true)
            .attr(Attribute.CHECKED, true);
        }
        rowCheckboxes.each((i, v) => {
          const $elem = $(v);
          if ($elem.attr(Attribute.CHECKED)) {
            checkboxBulkAction
              .prop(Attribute.INDETERMINATE, true)
              .attr(Attribute.INDETERMINATE, Attribute.INDETERMINATE);
            bulkActions.removeClass(ClassName.D_NONE);
            replacedElement.addClass(ClassName.D_NONE);
            return false;
          }
          if (i === checkboxBulkAction.length) {
            checkboxBulkAction
              .prop(Attribute.INDETERMINATE, false)
              .attr(Attribute.INDETERMINATE, false);
            checkboxBulkAction
              .prop(Attribute.CHECKED, false)
              .attr(Attribute.CHECKED, false);
            bulkActions.addClass(ClassName.D_NONE);
            replacedElement.removeClass(ClassName.D_NONE);
          }
          return true;
        });
      });
    });
  }
});
