
Ext.define('Ext.selection.CheckboxModel', {
    alias: 'selection.checkboxmodel',
    extend: 'Ext.selection.RowModel.Selector',

    mode: 'MULTI',


	storeColNameForHide:'',

    injectCheckbox: 0,

    checkOnly: false,

    headerWidth: 24,

    // private
    checkerOnCls: Ext.baseCSSPrefix + 'grid-hd-checker-on',

    bindComponent: function(view) {
        var me = this;

        me.sortable = false;
        me.callParent(arguments);
        if (!me.hasLockedHeader() || view.headerCt.lockedCt) {
            // if we have a locked header, only hook up to the first
            view.headerCt.on('headerclick', me.onHeaderClick, me);
            me.addCheckbox(true);
            me.mon(view.ownerCt, 'reconfigure', me.addCheckbox, me);
        }
    },

    hasLockedHeader: function(){
        var hasLocked = false;
        Ext.each(this.views, function(view){
            if (view.headerCt.lockedCt) {
                hasLocked = true;
                return false;
            }
        });
        return hasLocked;
    },


    addCheckbox: function(initial){
        var me = this,
            checkbox = me.injectCheckbox,
            view = me.views[0],
            headerCt = view.headerCt;

        if (checkbox !== false) {
            if (checkbox == 'first') {
                checkbox = 0;
            } else if (checkbox == 'last') {
                checkbox = headerCt.getColumnCount();
            }
            headerCt.add(checkbox,  me.getHeaderConfig());
        }

        if (initial !== true) {
            view.refresh();
        }
    },

   toggleUiHeader: function(isChecked) {
        var view     = this.views[0],
            headerCt = view.headerCt,
            checkHd  = headerCt.child('gridcolumn[isCheckerHd]');

        if (checkHd) {
            if (isChecked) {
                checkHd.el.addCls(this.checkerOnCls);
            } else {
                checkHd.el.removeCls(this.checkerOnCls);
            }
        }
    },

    onHeaderClick: function(headerCt, header, e) {
        if (header.isCheckerHd) {
            e.stopEvent();
            var isChecked = header.el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
            if (isChecked) {
                // We have to supress the event or it will scrollTo the change
                this.deselectAll(true,this.storeColNameForHide);
            } else {
                // We have to supress the event or it will scrollTo the change
                this.selectAll(true,this.storeColNameForHide);
            }
        }
    },

    getHeaderConfig: function() {
        var me = this;

        return {
            isCheckerHd: true,
            text : '&#160;',
            width: me.headerWidth,
            sortable: false,
            draggable: false,
            resizable: false,
            hideable: false,
            menuDisabled: true,
            dataIndex: '',
            cls: Ext.baseCSSPrefix + 'column-header-checkbox ',
            renderer: Ext.Function.bind(me.renderer, me),
            locked: me.hasLockedHeader()
        };
    },

  
    renderer: function(value, metaData, record, rowIndex, colIndex, store, view) {
		if(this.storeColNameForHide == ''){
			metaData.tdCls = Ext.baseCSSPrefix + 'grid-cell-special';
			return '<div class="' + Ext.baseCSSPrefix + 'grid-row-checker">&#160;</div>';

		}else{
			if(record.get(this.storeColNameForHide) == 0){
				metaData.tdCls = Ext.baseCSSPrefix + 'grid-cell-special';
				return '<div class="' + Ext.baseCSSPrefix + 'grid-row-checker">&#160;</div>';
			}else{
				return '';
			}
		}
    },

    // override
    onRowMouseDown: function(view, record, item, index, e) {
        view.el.focus();
        var me = this,
            checker = e.getTarget('.' + Ext.baseCSSPrefix + 'grid-row-checker');
            
        if (!me.allowRightMouseSelection(e)) {
            return;
        }

        // checkOnly set, but we didn't click on a checker.
        if (me.checkOnly && !checker) {
            return;
        }

        if (checker) {
            var mode = me.getSelectionMode();
            // dont change the mode if its single otherwise
            // we would get multiple selection
            if (mode !== 'SINGLE') {
                me.setSelectionMode('SIMPLE');
            }
            me.selectWithEvent(record, e);
            me.setSelectionMode(mode);
        } else {
            me.selectWithEvent(record, e);
        }
    },


    onSelectChange: function() {
        this.callParent(arguments);
        var shouldCount = 0;
		if(this.storeColNameForHide !== ''){
			for(var a =0; a < this.store.getCount();a++){
				if(this.store.getAt(a).get(this.storeColNameForHide) == 0){
					shouldCount = shouldCount +1;
				}
			}
		}else{
			shouldCount = this.store.getCount();
		}
        var hdSelectStatus = this.selected.getCount() === shouldCount;
        this.toggleUiHeader(hdSelectStatus);
    }
});

