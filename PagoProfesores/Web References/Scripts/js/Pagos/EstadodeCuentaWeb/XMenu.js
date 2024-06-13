
function XMenu() {
	this.SelectedTab = '';
	this.arrItems = [];
	this.onCLick = null;

	this.init = function (defaultTab, div_XMenu_id, f_onclick) {
		var parent = document.getElementById(div_XMenu_id);
		var idparent = parent.id;
		var count = parent.childNodes.length;
		var prefijo = $('#' + idparent).data('find-items');

		this.arrItems = [];
		var idToInvoke = null;
		for (var i = 0; i < count; i++) {
			if (parent.childNodes[i].childNodes.length > 0) {
				var subObj = parent.childNodes[i].childNodes[0];
				if (subObj.id != null && ('' + subObj.id).indexOf(prefijo) >= 0) {
					var panel = $('#' + subObj.id).data('panel');

					$('#' + panel).hide();
					if (subObj.id == defaultTab)
						idToInvoke = subObj.id;

					this.arrItems.push({
						id: subObj.id,
						panel: panel
					});
				}
			}
		}
		if (idToInvoke != null)
			this.tab_click(document.getElementById(idToInvoke));
		this.onCLick = f_onclick;
	};

	this.tab_click = function (obj, dataTableName) {
		var parent = obj.parentElement.parentElement;
		var idparent = parent.id;
		var count = parent.childNodes.length;
		var prefijo = $('#' + idparent).data('find-items');
		var panelToShow = null;

		this.SelectedTab = -1;
		for (var i = 0; i < this.arrItems.length; i++) {
			var subObj = document.getElementById(this.arrItems[i].id);
			var panel = this.arrItems[i].panel;
			if (panel != '')
				$('#' + panel).hide();
			if (subObj == obj) {
				this.SelectedTab = subObj.id;
				$('#' + subObj.id).removeClass('xbtn-off').removeClass('xbtn-blur');
				$('#' + subObj.id).addClass('xbtn-on').addClass('xbtn-focus');
				panelToShow = panel;
			}
			else {
				$('#' + subObj.id).removeClass('xbtn-on').removeClass('xbtn-focus');
				$('#' + subObj.id).addClass('xbtn-off').addClass('xbtn-blur');
			}
		}
		if (panel != '')
			$('#' + panelToShow).show();

		if (this.onCLick != null && (typeof this.onCLick) == 'function')
			this.onCLick(this.SelectedTab);
	};
	this.tab_over = function (obj) {
		if (obj.id != this.SelectedTab) {
			$('#' + obj.id).removeClass('xbtn-blur');
			$('#' + obj.id).addClass('xbtn-focus');
		}
	};
	this.tab_out = function (obj) {
		if (obj.id != this.SelectedTab) {
			$('#' + obj.id).removeClass('xbtn-focus');
			$('#' + obj.id).addClass('xbtn-blur');
		}
	};
}
