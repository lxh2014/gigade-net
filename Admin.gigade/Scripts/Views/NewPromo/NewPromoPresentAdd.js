editPresentFunction = function (row, store, o_event_id,timestart,timeend) {
    VipGroupStore.load();

    var p_editFrm = Ext.create('Ext.form.Panel', {
        id: 'p_editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/NewPromo/InestNewPromoPresent',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [


                {
                    xtype: 'combobox', //會員群組
                    editable: false,
                    hidden: false,
                    id: 'p_group_id',
                    fieldLabel: USERGROUP,
                    //allowBlank: false,
                    name: 'p_group_id',
                    hiddenName: 'group_id',
                    store: VipGroupStore,
                    lastQuery: '',
                    displayField: 'group_name',
                    valueField: 'group_id',
                    typeAhead: true,
                    forceSelection: false,
                    value: "0"
                },
               {
                   xtype: 'fieldcontainer',
                   combineErrors: true,
                   layout: 'hbox',
                   items: [
                     { xtype: 'displayfield', value: GIFTTIPE },
                     {
                         xtype: 'radiofield',
                         boxLabel: PRODUCT,
                         margin: '0 0 0 18',
                         name: 'state',
                         id: 'state1',
                         checked: true,
                         listeners: {
                             change: function (radio, newValue, oldValue) {
                                 var result = Ext.getCmp('state1').getValue();
                                 if (result) {
                                     Ext.getCmp('p_deduct_welfare').setValue(0);
                                     Ext.getCmp('p_deduct_welfare').hide();
                                     Ext.getCmp('p_ticket_name').setValue("0");
                                     Ext.getCmp('p_ticket_name').allowBlank = true;
                                     Ext.getCmp('p_ticket_name').setValue("");
                                     Ext.getCmp('p_ticket_name').hide();
                                     Ext.getCmp('p_gift_id').show();
                                     Ext.getCmp('p_product_name').show();
                                     Ext.getCmp('p_freight_price').show();
                                     Ext.getCmp('p_gift_amount').show();
                                 }
                                 else {
                                     Ext.getCmp('p_gift_id').setValue("");
                                     Ext.getCmp('p_product_name').setValue("");
                                     Ext.getCmp('p_freight_price').setValue(0);
                                     Ext.getCmp('p_gift_id').hide();
                                     Ext.getCmp('p_product_name').hide();
                                     Ext.getCmp('p_freight_price').hide();
                                     Ext.getCmp('p_gift_amount').hide();
                                     Ext.getCmp('p_deduct_welfare').show();
                                     Ext.getCmp('p_ticket_name').allowBlank = false;
                                     Ext.getCmp('p_ticket_name').show();
                                 }
                             }
                         }
                     },
                        {
                            xtype: 'radiofield',
                            boxLabel: BONUS,
                            margin: '0 0 0 10',
                            name: 'state',
                            id: 'state2'

                        },
                        {
                            xtype: 'radiofield',
                            boxLabel: DIYKONG,
                            margin: '0 0 0 10',
                            name: 'state',
                            id: 'state3'
                        }
                   ]
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: GIFTID,
                   id: 'p_gift_id',
                   name: 'p_gift_id',
                   submitValue: true,
                   listeners: {
                       change: function () {
                           var id = Ext.getCmp('p_gift_id').getValue();
                           Ext.Ajax.request({
                               url: "/NewPromo/GetProductnameById",
                               method: 'post',
                               type: 'text',
                               params: {
                                   id: Ext.getCmp('p_gift_id').getValue()
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(form.responseText);
                                   if (result.success) {
                                       msg = result.msg;
                                       Ext.getCmp("p_product_name").setValue(msg);
                                   } else {
                                       Ext.getCmp("p_product_name").setValue(NOPRODUCT);
                                   }
                               }
                           });
                       }
                   }
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: GIFTNAME,
                   id: 'p_product_name',
                   name: 'p_product_name',
                   allowBlank: false,
                   submitValue: true
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: GIFTNUM,
                   id: 'p_gift_amount',
                   name: 'p_gift_amount',
                   allowBlank: false,
                   submitValue: true,
                   value: 1,
                   minValue: 1
               },
                 {
                     xtype: 'textfield',
                     fieldLabel: GIFTNAME,
                     id: 'p_ticket_name',
                     name: 'p_ticket_name',
                     allowBlank: true,
                     submitValue: true,
                     hidden: true
                 },
               {
                   xtype: 'numberfield',
                   fieldLabel: BONUSWELFARE,
                   id: 'p_deduct_welfare',
                   name: 'p_deduct_welfare',
                   allowBlank: false,
                   submitValue: true,
                   minValue: 0,
                   hidden: true,
                   value: 0
               },

               {
                   xtype: 'numberfield',
                   fieldLabel: FREIGHTPRICE,
                   id: 'p_freight_price',
                   name: 'p_freight_price',
                   allowBlank: false,
                   submitValue: true,
                   minValue: 0,
                   value: 0
               }
               ,
               {
                   xtype: "datetimefield",
                   fieldLabel: DATESTART,
                   editable: false,
                   id: 'p_start',
                   name: 'p_start',
                   anchor: '95%',
                   format: 'Y-m-d H:i:s',
                   width: 150,
                   allowBlank: false,
                   submitValue: true,
                   value: Tomorrow(),
                   time: { hour: 00, min: 00, sec: 00 },
                   listeners: {
                       select: function (a, b, c) {
                           var start = Ext.getCmp("p_start");
                           var end = Ext.getCmp("p_end");
                           if (end.getValue() < start.getValue()) {
                               var start_date = start.getValue();
                               Ext.getCmp('p_end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                           }
                       }
                   }
               },
               {
                   xtype: "datetimefield",
                   fieldLabel: DATEEND,
                   editable: false,
                   id: 'p_end',
                   anchor: '95%',
                   name: 'p_end',
                   format: 'Y-m-d H:i:s',
                   allowBlank: false,
                   submitValue: true,
                   time: { hour: 23, min: 59, sec: 59 },
                   value: setNextMonth(Tomorrow(), 1),
                   listeners: {
                       select: function (a, b, c) {
                           var start = Ext.getCmp("p_start");
                           var end = Ext.getCmp("p_end");
                           if (end.getValue() < start.getValue()) {
                               var end_date = end.getValue();
                               Ext.getCmp('p_start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                           }
                       }
                   }
               }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();

                if (form.isValid()) {
                    if (Ext.getCmp('state1').getValue() == true) {
                        if (Ext.getCmp('p_product_name').getValue() != "" && Ext.getCmp('p_product_name').getValue() != "沒有該商品信息！") {
                            form.submit({
                                params: {
                                    row_id: "",
                                    this_event_id: "",
                                    group_id: Ext.getCmp('p_group_id').getValue(),
                                    state1: Ext.getCmp('state1').getValue(),
                                    state2: Ext.getCmp('state2').getValue(),
                                    state3: Ext.getCmp('state3').getValue(),
                                    gift_id: Ext.getCmp('p_gift_id').getValue(),
                                    product_name: Ext.htmlEncode(Ext.getCmp('p_product_name').getValue()),
                                    ticket_name: Ext.htmlEncode(Ext.getCmp('p_ticket_name').getValue()),
                                    deduct_welfare: Ext.htmlEncode(Ext.getCmp('p_deduct_welfare').getValue()),
                                    freight_price: Ext.htmlEncode(Ext.getCmp('p_freight_price').getValue()),
                                    valid_end: Ext.getCmp('p_end').getValue(),
                                    valid_start: Ext.getCmp('p_start').getValue(),
                                    gift_amount: Ext.getCmp('p_gift_amount').getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    if (result.success) {
                                        if (o_event_id != null) {
                                            o_event_id.setValue(result.event_id);
                                        }
                                        if (store != null) {
                                            PromoRresentStore.load();
                                        }
                                        p_editWins.close();

                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, PRODTIP);
                        }
                    }
                    else {
                        form.submit({
                            params: {
                                row_id: "",
                                this_event_id: "",
                                state1: Ext.getCmp('state1').getValue(),
                                state2: Ext.getCmp('state2').getValue(),
                                state3: Ext.getCmp('state3').getValue(),
                                gift_id: Ext.getCmp('p_gift_id').getValue(),
                                group_id: Ext.getCmp('p_group_id').getValue(),
                                product_name: Ext.htmlEncode(Ext.getCmp('p_product_name').getValue()),
                                ticket_name: Ext.htmlEncode(Ext.getCmp('p_ticket_name').getValue()),
                                deduct_welfare: Ext.htmlEncode(Ext.getCmp('p_deduct_welfare').getValue()),
                                freight_price: Ext.htmlEncode(Ext.getCmp('p_freight_price').getValue()),
                                valid_start: Ext.getCmp('p_start').getValue(),
                                valid_end: Ext.getCmp('p_end').getValue(),
                                gift_amount: Ext.getCmp('p_gift_amount').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                if (result.success) {
                                    if (o_event_id != null) {
                                        o_event_id.setValue(result.event_id);
                                    }
                                    if (store != null) {
                                        PromoRresentStore.load();
                                    }
                                    p_editWins.close();
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }
        }]
    });
    var p_editWins = Ext.create('Ext.window.Window', {
        title: GIFTSET,
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'p_editWins',
        width: 400,
        height: 350,
        layout: 'fit',
        items: [p_editFrm],
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
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('p_editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row == null) {
                    p_editFrm.getForm().reset(); //如果是添加的話
                }


            }
        }
    });
    p_editWins.show();
    function Tomorrow() {
        var d;
        var dt;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();
        dt = new Date(s);
        dt.setDate(dt.getDate() + 1);
        return dt;                                 // 返回日期。
    }

    function setNextMonth(source, n) {
        var s = new Date(source);
        s.setMonth(s.getMonth() + n);
        if (n < 0) {
            s.setHours(0, 0, 0);
        }
        else if (n > 0) {
            s.setHours(23, 59, 59);
        }
        return s;
    }
}

