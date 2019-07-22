'use strict';

import utils from './Utils';


/*-----------------------------------------------
|   Detector
-----------------------------------------------*/

utils.$document.ready(() => {
  if (window.is.opera()) utils.$html.addClass('opera');
  if (window.is.mobile()) utils.$html.addClass('mobile');
  if (window.is.firefox()) utils.$html.addClass('firefox');
  if (window.is.safari()) utils.$html.addClass('safari');
  if (window.is.ios()) utils.$html.addClass('ios');
  if (window.is.ie()) utils.$html.addClass('ie');
  if (window.is.edge()) utils.$html.addClass('edge');
  if (window.is.chrome()) utils.$html.addClass('chrome');
  if (utils.nua.match(/puppeteer/i)) utils.$html.addClass('puppeteer');
  if (window.is.mac()) utils.$html.addClass('osx');
});




