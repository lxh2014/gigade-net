editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/MemberEvent/SaveMemberLevel',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '編號',
                id: 'rowID',
                name: 'rowID',
                hidden: true,
            },
            {
                xtype: 'textfield',
                fieldLabel: '級別名稱',
                id: 'ml_name',
                name: 'ml_name',
                allowBlank: false,
            },
            {
                xtype: 'textfield',
                fieldLabel: '級別代碼',
                id: 'ml_code',
                name: 'ml_code',
                allowBlank: false,
                listeners: {
                    //'change': function () {
                    //    if (Ext.getCmp('oldCode').getValue() != Ext.getCmp('ml_code').getValue()) {
                    //        Ext.Ajax.request({
                    //            url: '/MemberEvent/DistinctCode',
                    //            params: {
                    //                ml_code: Ext.getCmp('ml_code').getValue(),
                    //            },
                    //            success: function (form, action) {
                    //                var result = Ext.decode(form.responseText);
                    //                if (!result.success) {
                    //                    Ext.getCmp('ml_code').setValue("");
                    //                    Ext.Msg.alert("提示信息", "級別代碼不允許重複");
                    //                }
                    //            }
                    //        });
                    //    }
                    //}
                },
            },
            {
                xtype: 'displayfield',
                id: 'oldCode',
                fieldLabel: '老代碼',
                hidden: true,
            },
            {
                xtype: 'fieldset',
                columnWidth: 0.5,
                title: '級別條件',
                //collapsible: true,//可摺疊
                defaultType: 'textfield',
                defaults: { anchor: '100%' },
                layout: 'anchor',
                items: [
                               {
                                   xtype: 'fieldcontainer',
                                   layout: 'hbox',
                                   items: [
                                               {
                                                   xtype: 'numberfield',
                                                   fieldLabel: '累積購物金額',
                                                   id: 'ml_minimal_amount',
                                                   allowBlank: false,
                                                 width:205,
                                                   name: 'ml_minimal_amount',
                                                   allowDecimals: false,
                                                   minValue: 0,
                                               },
                                               {
                                                   xtype: 'displayfield',
                                                   value: '~',
                                               },
                                               {
                                                   xtype: 'numberfield',
                                                
                                                   id: 'ml_max_amount',
                                                   allowBlank: false,
                                                   width: 100,
                                                   name: 'ml_max_amount',
                                                   allowDecimals: false,
                                                   minValue: 0,
                                               },
                                   ]
                               },
                ]
            },

    {
        xtype: 'fieldset',
        //title: '其他條件',
        layout: 'anchor',
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                     {
                         xtype: 'checkbox',
                         id: 'ischecked',
                         listeners: {
                             change: function () {
                                 //if (Ext.getCmp('ischecked').getValue() == true) {
                                 //    Ext.getCmp('ml_month_seniority').setDisabled(false);
                                 //}
                                 //else {
                                 //    Ext.getCmp('ml_month_seniority').setDisabled(true);
                                 //}
                             }
                         }
                     },
                    {
                        xtype: 'displayfield',
                        value: '其他條件',
                    },

                ]

            },
    {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [
                    {
                        xtype: 'numberfield',
                        fieldLabel: '加入會員時間',
                        id: 'ml_month_seniority',
                        name: 'ml_month_seniority',
                        disabled: true,
                        allowDecimals: false,
                        minValue: 0,
                    },
                    {
                        xtype: 'displayfield',
                        value: '月(含)以上',
                    },
        ]
    },

            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                            {
                                xtype: 'numberfield',
                                fieldLabel: '前次購買時間',
                                id: 'ml_last_purchase',
                                name: 'ml_last_purchase',
                                disabled: true,
                                allowDecimals: false,
                                minValue: 0,
                            },
                            {
                                xtype: 'displayfield',
                                value: '月(含)以內',
                            },
                ]
            },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
                        {
                            xtype: 'numberfield',
                            fieldLabel: '消費次數至少',
                            id: 'ml_minpurchase_times',
                            name: 'ml_minpurchase_times',
                            minValue: 0,
                            allowDecimals: false,
                            disabled: true,
                        },
                        {
                            xtype: 'displayfield',
                            value: '次',
                        },
            ]
        },

        ]
    },
       {
           xtype: 'fieldcontainer',
           layout: 'hbox',
           items: [
                         {
                             xtype: 'numberfield',
                             fieldLabel: '生日禮金',
                             id: 'ml_birthday_voucher',
                             name: 'ml_birthday_voucher',
                             allowBlank: false,
                             minValue: 0,
                             allowDecimals: false,
                             value: 0
                         },
                         {
                             xtype: 'displayfield',
                             value: '元',
                         },
           ]
       },
              {
                  xtype: 'fieldcontainer',
                  layout: 'hbox',
                  items: [
                                {
                                    xtype: 'numberfield',
                                    fieldLabel: '免運券數量',
                                    id: 'ml_shipping_voucher',
                                    name: 'ml_shipping_voucher',
                                    allowBlank: false,
                                    minValue: 0,
                                    maxValue: 1000,
                                    allowDecimals: false,
                                 
                                    value: 0
                                },
                                {
                                    xtype: 'displayfield',
                                    value: '張',
                                },
                  ]
              },
            {

                xtype: 'numberfield',
                fieldLabel: '級別排序',
                id: 'ml_seq',
                name: 'ml_seq',
                allowBlank: false,
                allowDecimals: false,
                minValue: 0

            },
                        {

                            xtype: 'numberfield',
                            fieldLabel: '老排序',
                            id: 'old_ml_seq',
                            name: 'old_ml_seq',
                            //allowBlank: false,
                            hidden: true,

                        }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    if (Ext.getCmp('ml_minimal_amount').getValue() > Ext.getCmp('ml_max_amount').getValue())
                    {
                        Ext.Msg.alert("提示信息", "購買金額上限不能大於購買金額下限！");
                        return;
                    }
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    myMask.show();
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                rowID: Ext.htmlEncode(Ext.getCmp('rowID').getValue()),
                                ml_name: Ext.htmlEncode(Ext.getCmp('ml_name').getValue()),
                                ml_code: Ext.htmlEncode(Ext.getCmp('ml_code').getValue()),
                                ml_minimal_amount: Ext.htmlEncode(Ext.getCmp('ml_minimal_amount').getValue()),
                                ml_max_amount: Ext.htmlEncode(Ext.getCmp('ml_max_amount').getValue()),
                                other_condition: Ext.htmlEncode(Ext.getCmp('ml_minimal_amount').getValue()),
                                ml_month_seniority: Ext.htmlEncode(Ext.getCmp('ml_month_seniority').getValue()),
                                ml_last_purchase: Ext.htmlEncode(Ext.getCmp('ml_last_purchase').getValue()),
                                ml_minpurchase_times: Ext.htmlEncode(Ext.getCmp('ml_minpurchase_times').getValue()),
                                ml_birthday_voucher: Ext.htmlEncode(Ext.getCmp('ml_minpurchase_times').getValue()),
                                ml_seq: Ext.htmlEncode(Ext.getCmp('ml_seq').getValue()),
                                old_ml_seq: Ext.htmlEncode(Ext.getCmp('ml_seq').getValue()),
                                old_ml_code: Ext.htmlEncode(Ext.getCmp('oldCode').getValue()),
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg == "0") {
                                        Ext.Msg.alert(INFORMATION, "級別排序不允許重複 ");
                                        Ext.getCmp('ml_seq').setValue();
                                    }
                                    else if (result.msg == "2") {
                                        Ext.Msg.alert(INFORMATION, "級別代碼不允許重複 ");
                                        Ext.getCmp('ml_code').setValue('');
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }

                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    store.load();
                                    editWin.close();
                                }
                            },
                            failure: function () {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                            }
                        });
                    }
                }
            },
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '會員級別新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        height: 440,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
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
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    initRow(row);
                }
                else {
                    Ext.Ajax.request({
                        url: '/MemberEvent/MaxMLSeq',
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.getCmp('ml_seq').setValue(result.ml_seq);
                            }
                        }

                    });
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initRow(row) {
        Ext.getCmp('ml_seq').show(true);
        //Ext.getCmp('ml_seq').allowBlank = false;
        Ext.getCmp('oldCode').setValue(row.data.ml_code);
        Ext.getCmp('old_ml_seq').setValue(row.data.ml_seq);

    }
}