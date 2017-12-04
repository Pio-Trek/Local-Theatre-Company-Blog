var images = ['bg1.jpg', 'bg2.jpg', 'bg3.jpg', 'bg4.jpg', 'bg5.jpg', 'bg6.jpg', 'bg7.jpg', 'bg8.jpg', 'bg9.jpg', 'bg10.jpg', 'bg11.jpg', 'bg12.jpg', 'bg13.jpg', 'bg14.jpg'];

var myNumber = Math.floor((Math.random() * 14));


$(document).ready(function () {

    document.getElementById('backgroundImage').style.backgroundImage = 'url(/images/' + images[myNumber] + ')';
});
