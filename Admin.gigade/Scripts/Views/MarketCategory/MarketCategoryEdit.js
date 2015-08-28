editFunction = function (Row, store, fatherId, fatherName) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/MarketCategory/SaveMarketCategory',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
                {
                    xtype: 'textfield',
                    fieldLabel: 'ID',
                    id: 'market_category_id',
                    name: 'market_category_id',
                    hidden: true
                },
                 {
                     xtype: 'displayfield',
                     fieldLabel: MARKET_CATEGORY_FATHER,
                     id: 'market_category_father',
                     name: 'market_category_father',
                     value: "[" + fatherId + "]" + fatherName
                 },
               {
                   xtype: 'numberfield',
                   fieldLabel: CATEGORY_CODE,
                   allowBlank: false,
                   allowDecimals: false,
                   id: 'market_category_code',
                   minValue: 0,
                   maxLength: 9,    
                   name: 'market_category_code'
               },
                {
                    xtype: 'textfield',
                    fieldLabel: MARKET_CATEGORY_NAMES,
                    id: 'market_category_name',
                    name: 'market_category_name'
                },
                  {//
                      xtype: 'numberfield',
                      fieldLabel: CATEGORY_SORT,
                      allowBlank: false,
                      value: 0,
                      allowDecimals: false,
                      minValue: 0,
                      id: 'market_category_sort',
                      name: 'market_category_sort'
                  },
                {
                    xtype: 'radiogroup',
                    hidden: false,
                    id: 'market_category_status',
                    name: 'market_category_status',
                    fieldLabel: WhETHER_ENABLE,
                    colName: 'market_category_status',
                    width: 400,
                    defaults: {
                        name: 'category_status',
                        margin: '0 8 0 0'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                            {
                                boxLabel: ISSTATUS, id: 'isStatus', inputValue: '1', checked: true
                            },
                            {
                                boxLabel: NOSTATUS, id: 'noStatus', inputValue: '0'
                            }
                    ]
                }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var count = 0;
                    if (!Row) {//新增時判定market_category_code是否存在
                        Ext.Ajax.request({
                            url: '/MarketCategory/GetMarketCategoryCount',
                            method: 'post',
                            async: false,
                            params: { code: Ext.htmlEncode(Ext.getCmp("market_category_code").getValue()) },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    count = result.count;
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                        if (count > 0) {
                            Ext.Msg.alert(INFORMATION, ALEADY_EXIST);
                            return;
                        }
                    }
                    form.submit({
                        params: {
                            market_category_id: Ext.htmlEncode(Ext.getCmp("market_category_id").getValue()),
                            market_category_code: Ext.htmlEncode(Ext.getCmp("market_category_code").getValue()),
                            market_category_name: Ext.htmlEncode(Ext.getCmp("market_category_name").getValue()),
                            market_category_sort: Ext.htmlEncode(Ext.getCmp("market_category_sort").getValue()),
                            market_category_status: Ext.htmlEncode(Ext.getCmp("market_category_status").getValue().category_status),
                            market_category_father_id: Ext.htmlEncode(fatherId)

                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                store.load({
                                    params: {
                                        father_id: fatherId
                                    }
                                });
                                editWin.close();
                            } else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });


                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: CATEGORY_NAME,
        id: 'editWin',
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 380,
        height: 300,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
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
         }],
        listeners: {
            'show': function () {
                if (Row) {
                    editFrm.getForm().loadRecord(Row); //如果是編輯的話
                    Ext.getCmp("market_category_code").setDisabled(true);
                    if (Row.data.market_category_status == 0) {
                        Ext.getCmp("noStatus").setValue(true);
                    }
                    else {
                        Ext.getCmp("isStatus").setValue(true);
                    }
                }
                else {
                    editFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
}

