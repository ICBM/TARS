$(document).ready(function(){
	
/*	$('#clickme').click(function() {$('#clickme').fadeOut('fast', function(){}).attr("src", "images/bridgeofglass.jpg").fadeIn('fast',function(){});});	
	*/
	/*
	var nextImage = function{img, next){
		$(img).click(function(){
			$(img).fadeOut('fast', function(){}).attr('src', next).fadeIn('fast',function(){});
		});
	}
	

	var setFadeClick = function(name, src_clicked, src_unclicked){
			$(name).click(function(){
				if($(name).attr('src') == src_unclicked){
					$(name).fadeOut('fast', function(){}).attr('src', src_clicked).fadeIn('fast',function(){});
				}
				else {
					$(name).fadeOut('fast', function(){}).attr('src', src_unclicked).fadeIn('fast',function(){});
				}
			});	
	}	
	*/
	
	//setFadeClick("#switch_img","images/bridgeofglass.jpg","images/bridgeofglass_text.png");
	
	$('#garden').click(function(){
			$('#switch_img').fadeOut('fast', function(){}).attr('src', "images/bridgeofglass_text.png").fadeIn('fast',function(){});
			$('.notch').css('margin-left', '260px');
	});
	
	$('#tree').click(function(){
			$('#switch_img').fadeOut('fast', function(){}).attr('src', "images/tractor.png").fadeIn('fast',function(){});
			$('.notch').css('margin-left', '480px');
	});
	
	$('#other').click(function(){
			$('#switch_img').fadeOut('fast', function(){}).attr('src', "images/rails.png").fadeIn('fast',function(){});
			$('.notch').css('margin-left', '690px');
	});

	
	
	
});