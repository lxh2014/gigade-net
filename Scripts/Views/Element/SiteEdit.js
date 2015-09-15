editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Element/SaveSiteInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: SITEID,
            id: 'site_id',
            name: 'site_id',
            submitValue: true,
            hidden: true
        },
        {
            xtype: 'textfield',
            name: 'site_name',
            id: 'site_name',
            submitValue: true,
            allowBlank: false,
            maxLength: 25,
            fieldLabel: SITENAME
        }, {
            xtype: 'textfield',
            name: 'domain',
            id: 'domain',
            maxLength: 25,
            submitValue: true,
            fieldLabel: DOMAIN
        },
        {
            xtype: 'combobox', 
            name: 'cart_delivery',
            id: 'cart_delivery',
            editable: false,
            fieldLabel: SCATEDELIVERY,
            store: SiteStore,
            // queryMode: 'local',
            lastQuery:'',
            submitValue: true,
            displayField: 'Site_Name',
            valueField: 'Site_Id',
            typeAhead: true,
            forceSelection: false,
            value: 1
        }, 
       {
           xtype: 'numberfield',
           name: 'online_user',
           id: 'online_user',
           minValue: 0,
           maxLength: 9,
           hidden:true,
           submitValue: true,
           fieldLabel: ONLINEUSER,
           listeners: {
               afterRender: function (combo) {
                   combo.setValue(0);
               }
           }
       },
       {
           xtype: 'numberfield',
           name: 'max_user',
           id: 'max_user',
           minValue: 0,
           maxLength: 9,
           submitValue: true,
           fieldLabel: MAXUSER,
           listeners: {
               afterRender: function (combo) {
                   combo.setValue(0);
               }
           }
       },
        {
            xtype: 'textfield',
            name: 'page_location',
            id: 'page_location',
            fieldLabel: PAGELOCATION,
            allowBlank: true,
            submitValue: true
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            site_id: Ext.htmlEncode(Ext.getCmp('site_id').getValue()),
                            site_name: Ext.htmlEncode(Ext.getCmp('site_name').getValue()),
                            domain: Ext.htmlEncode(Ext.getCmp('domain').getValue()),
                            cart_delivery: Ext.htmlEncode(Ext.getCmp('cart_delivery').getValue()),
                            online_user: Ext.htmlEncode(Ext.getCmp('online_user').getValue()),
                            max_user: Ext.htmlEncode(Ext.getCmp('max_user').getValue()),
                            page_location: Ext.htmlEncode(Ext.getCmp('page_location').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS + result.msg);
                                SiteStore.load();
                                SitesStore.load();
                                editWin.close();
                                
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE + result.msg);
                                SiteStore.load();
                                SitesStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, FAILURE + result.msg);
                            SitesStore.load();
                           // SiteStore.load();
                            editWin.close();
                            SiteStore.load();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: SITETITLE,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: ISCLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
            }
        ],
        listeners: {
            'show': function () {
                    editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
}
