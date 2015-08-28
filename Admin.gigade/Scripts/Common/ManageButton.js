Ext.define("Ext.ux.FunctionButton", {
    extend: "Ext.container.Container",
    alias: 'widget.ManageButton',
    "initComponent": function () {
        var self = this;
        self.setaddshow = function (bol) { if (bol == true) { Ext.getCmp("add_button").show(); }; if (bol == false) { Ext.getCmp("add_button").hide(); } };
        self.setupdateshow = function (bol) { if (bol == true) { Ext.getCmp("update_button").show(); }; if (bol == false) { Ext.getCmp("update_button").hide(); } };
        self.setdeleteshow = function (bol) { if (bol == true) { Ext.getCmp("delete_button").show(); }; if (bol == false) { Ext.getCmp("delete_button").hide(); } };
        self.setaddtext = function (text) { Ext.getCmp("add_button").setText(text); };
        self.setupdatetext = function (text) { Ext.getCmp("update_button").setText(text); };
        self.setdeletetext = function (text) { Ext.getCmp("delete_button").setText(text); };
        self.setadddisable = function (bol) { Ext.getCmp("add_button").setDisabled(bol); };
        self.setupdatedisable = function (bol) { Ext.getCmp("add_button").setDisabled(bol); };
        self.setdeletedisable = function (bol) { Ext.getCmp("add_button").setDisabled(bol); };
        Ext.apply(self, {
            addtext: self.addtext,   //add控件text值
            addable: self.addable,   //add控件是否啟用
            addhidden: self.addhidden,    //add控件是否顯示
            addhandler: self.addhandler,
            updatetext: self.updatetext,
            updateable: self.updateable,
            updatehidden: self.updatehidden,
            updatehandler: self.updatehandler,
            deletetext: self.deletetext,
            deleteable: self.deleteable,
            deletehidden: self.deletehidden,
            deletehandler: self.deletehandler
        });
        self.items = [
            {
                xtype: "button",
                id: "add_button",
                hidden: self.addhidden,
                handler: self.addhandler,
                iconCls: 'ui-icon ui-icon-add',
                listeners: {
                    //加載控件狀態
                    added: function () {
                        if (self.addtext && self.addtext != "") {
                            Ext.getCmp("add_button").setText(self.addtext);
                        } else {
                            self.setaddshow(false);
                        }
                        if (self.addable) {
                            Ext.getCmp("add_button").setDisabled(true);
                        }
                    }
                }
            },
            {
                xtype: "button",
                id: "update_button",
                hidden: self.updatehidden,
                handler: self.updatehandler,
                iconCls: 'ui-icon ui-icon-pencil',
                listeners: {
                    //加載控件狀態
                    added: function () {
                        if (self.updatetext && self.updatetext != "") {
                            Ext.getCmp("update_button").setText(self.updatetext);
                        } else {
                            self.setupdateshow(false);
                        }
                        if (self.updateable) {
                            Ext.getCmp("update_button").setDisabled(true);
                        }
                    }
                }
            },
            {
                xtype: "button",
                id: "delete_button",
                hidden: self.deletehidden,
                handler: self.deletehandler,
                iconCls: 'ui-icon ui-icon-delete',
                listeners: {
                    //加載控件狀態
                    added: function () {
                        if (self.deletetext && self.deletetext != "") {
                            Ext.getCmp("delete_button").setText(self.deletetext);
                        } else {
                            self.setdeleteshow(false);
                        }
                        if (self.deleteable) {
                            Ext.getCmp("delete_button").setDisabled(true);
                        }
                    }
                }
            }
        ],
        self.callParent();
    },
});