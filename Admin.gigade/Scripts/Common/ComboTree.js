Ext.define("Ext.ux.comboboxtree", {
    extend: "Ext.form.field.Picker",
    requires: ["Ext.tree.Panel"],
    alias: 'widget.combotree',
    "initComponent": function () {
        var self = this;
        Ext.apply(self, {
            fieldLabel: self.fieldLabel,
            labelWidth: self.labelWidth,
            store: self.store
        });
        self.callParent();
    },
    "createPicker": function () {
        var self = this;
        var store = this.store;
        self.picker = new Ext.tree.Panel({
            height: 250,
            autoScroll: true,
            floating: true,
            focusOnToFront: true,
            shadow: true,
            //ownerCt: this.ownerCt,
            useArrows: true,
            store: store,
            rootVisible: false,
            resizable: true
        });
        self.picker.on({
            "itemdblclick": function (combotree, rec) {
                self.picker.hide();
            },
            "itemclick": function (combotree, rec) {
                self.setValue(rec.get('text'));

                Ext.getCmp('' + self.id + '_hide').setValue(rec.get('id'));
            }
        });
        return self.picker;
    },
    "alignPicker": function () {
        var me = this, picker, isAbove, aboveSfx = '-above';
        if (this.isExpanded) {
            picker = me.getPicker();
            if (me.matchFieldWidth) {
                picker.setWidth(me.bodyEl.getWidth());
            }
            if (picker.isFloating()) {
                picker.alignTo(me.inputEl, "", me.pickerOffset); // ""->tl   
                isAbove = picker.el.getY() < me.inputEl.getY();
                me.bodyEl[isAbove ? 'addCls' : 'removeCls'](me.openCls
          + aboveSfx);
                picker.el[isAbove ? 'addCls' : 'removeCls'](picker.baseCls
          + aboveSfx);
            }
        }
    }
});
