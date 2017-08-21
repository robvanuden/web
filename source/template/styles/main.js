var $item1 = $('#ide-support-carousel .item'); 
var $item2 = $('#default-settings-carousel .item'); 
var $item3 = $('#adaptive-logging-carousel .item'); 
var $item4 = $('#code-generation-carousel .item'); 
$item1.eq(0).addClass('active');
$item2.eq(0).addClass('active');
$item3.eq(0).addClass('active');
$item4.eq(0).addClass('active');


$('#auto-completion').on('click', function() {
  $('#ide-support-carousel').carousel(0)
});
$('#navigation').on('click', function() {
  $('#ide-support-carousel').carousel(1)
});
$('#debugging').on('click', function() {
  $('#ide-support-carousel').carousel(2)
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
$('#code-generation-carousel').carousel({
  interval: 6000
})