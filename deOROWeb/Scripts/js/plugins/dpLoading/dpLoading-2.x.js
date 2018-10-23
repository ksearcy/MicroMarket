if(dpUI===undefined)var dpUI={data:{},helper:{},options:{},versions:{}};
dpUI.versions.loading = "2.2.0";
dpUI.loading = {
	start: function(text,gif,timeout,timeout_func){
		if(text===undefined)text="";
		if(gif===undefined||gif===true)gif="<i class='fa fa-spinner fa-spin'></i>";
		if($(".dpui-loading").size()>0)dpUI.loading.stop();
		var html = "<div class='dpui-loading'><div class='dpui-loading-inner'>";
		if(gif)html+="<div class='dpui-loading-gif'>"+gif+"</div>";
		html+="<div class='dpui-loading-text'>"+text+"</div></div></div>";
		$("body").addClass("noscroll").append(html);
		if(timeout){
			if(timeout_func===undefined)timeout_func=function(){};
			$(".dpui-loading")[0].timeoutid = window.setTimeout(function(){
				$(".dpui-loading").remove();
				$("body").removeClass("noscroll");
				timeout_func();
			}, timeout);
		}
	},
	stop: function(){
		if($(".dpui-loading")[0].timeoutid)window.clearTimeout($(".dpui-loading")[0].timeoutid);
		$(".dpui-loading").remove();
		$("body").removeClass("noscroll");
	},
};
(function($){
	$.fn.dpLoading = function(action, text, timeout, timeout_func){
		action = action.toLowerCase();
		if(action=="start")dpUI.loading.start(text, timeout, timeout_func);
		else dpUI.loading.stop();
	}
}(jQuery));