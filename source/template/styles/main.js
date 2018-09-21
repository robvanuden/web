var $item1 = $('#ide-support-carousel .item'); 
var $item2 = $('#default-settings-carousel .item'); 
var $item3 = $('#adaptive-logging-carousel .item'); 
var $item4 = $('#code-generation-carousel .item'); 
var $item5 = $('#global-extension-carousel .item'); 
$item1.eq(0).addClass('active');
$item2.eq(0).addClass('active');
$item3.eq(0).addClass('active');
$item4.eq(0).addClass('active');
$item5.eq(0).addClass('active');


$('#solution-view').on('click', function() {
  $('#ide-support-carousel').carousel(0)
});
$('#code-completion').on('click', function() {
  $('#ide-support-carousel').carousel(1)
});
$('#debugging').on('click', function() {
  $('#ide-support-carousel').carousel(2)
});

$('#global-tool').on('click', function() {
  $('#global-extension-carousel').carousel(0)
});
$('#command-palette').on('click', function() {
  $('#global-extension-carousel').carousel(1)
});
$('#alt-enter').on('click', function() {
  $('#global-extension-carousel').carousel(2)
});

$('#references').on('click', function() {
  $('#code-generation-carousel').carousel(0)
});
$('#metadata').on('click', function() {
  $('#code-generation-carousel').carousel(1)
});
$('#schema').on('click', function() {
  $('#code-generation-carousel').carousel(2)
});

$('#ide-support-carousel').carousel({
    interval: 6000
})
$('#global-extension-carousel').carousel({
    interval: 6000
})
$('#code-generation-carousel').carousel({
  interval: 6000
})
