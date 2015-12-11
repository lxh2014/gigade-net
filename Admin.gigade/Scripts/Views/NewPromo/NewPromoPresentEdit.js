VipGroupStore.load();
EventTypeStore.load();
editPresentFunction = function (row, store, o_event_id) {

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
                   xtype: 'displayfield',
                   fieldLabel: "trow_id",
                   id: 'trow_id',
                   name: 'trow_id',
                   hidden: true
               },
               {
                    xtype: 'displayfield',
                    fieldLabel: "event_id",
                    id: 'event_id',
                    name: 'event_id',
                    hidden: true
                },
                {
                    xtype: 'combobox', //會員群組
                    editable: false,
                    hidden: false,
                    id: 'tgroup_id',
                    fieldLabel: USERGROUP,
                    //allowBlank: false,
                    name: 'tgroup_id',
                    hiddenName: 'tgroup_id',
                    store: VipGroupStore,
                    lastQuery: '',
                    displayField: 'group_name',
                    valueField: 'group_id',
                    typeAhead: true,
                    forceSelection: false,
                    value:"0"
                },
                {
                    xtype: 'combobox',
                    editable: false,
                    hidden: false,
                    id: 'event_type',
                    fieldLabel: '促銷類型',
                    name: 'event_type',
                    store: EventTypeStore,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    typeAhead: true,
                    lastQuery: '',
                    forceSelection: false,
                    value:0
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
                                     Ext.getCmp('deduct_welfare').setValue(0);
                                     Ext.getCmp('deduct_welfare').hide();
                                     Ext.getCmp('welfare_mulriple').setValue(0);
                                     Ext.getCmp('welfare_mulriple').hide();
                                     Ext.getCmp('ticket_name').setValue("0");
                                     Ext.getCmp('ticket_name').allowBlank = true;
                                     Ext.getCmp('ticket_name').setValue("");
                                     Ext.getCmp('ticket_name').hide();
                                     Ext.getCmp('gift_id').show();
                                     Ext.getCmp('product_name').show();
                                     Ext.getCmp('freight_price').show();
                                     Ext.getCmp('gift_amount').show();
                                     Ext.getCmp('bonus_expire_day').hide();
                                 }
                                 else {

                                     Ext.getCmp('gift_id').setValue("");
                                     Ext.getCmp('product_name').setValue("");
                                     Ext.getCmp('freight_price').setValue(0);
                                     Ext.getCmp('gift_id').hide();
                                     Ext.getCmp('product_name').hide();
                                     Ext.getCmp('freight_price').hide();
                                     Ext.getCmp('gift_amount').hide();
                                     Ext.getCmp('deduct_welfare').show();
                                     Ext.getCmp('ticket_name').allowBlank = false;
                                     Ext.getCmp('ticket_name').show();
                                     Ext.getCmp('bonus_expire_day').show();
                                 }
                             }
                         }
                     },
                        {
                            xtype: 'radiofield',
                            boxLabel: BONUS,
                            margin: '0 0 0 10',
                            name: 'state',
                            id: 'state2',
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    if (Ext.getCmp('state2').getValue()) {
                                        Ext.getCmp('welfare_mulriple').show();
                                    }
                                    else {
                                        Ext.getCmp('welfare_mulriple').setValue(1);
                                        Ext.getCmp('welfare_mulriple').hide();
                                    }
                                }
                            }

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
                   id: 'gift_id',
                   name: 'gift_id',
                   submitValue: true,
                   allowDecimals: false,
                   listeners: {
                       change: function () {
                           var id = Ext.getCmp('gift_id').getValue();
                           Ext.Ajax.request({
                               url: "/NewPromo/GetProductnameById",
                               method: 'post',
                               type: 'text',
                               params: {
                                   id: Ext.getCmp('gift_id').getValue()
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(form.responseText);
                                   if (result.success) {
                                       msg = result.msg;
                                       Ext.getCmp("product_name").setValue(msg);
                                   } else {
                                       Ext.getCmp("product_name").setValue(NOPRODUCT);
                                   }
                               }
                           });
                       }
                   }
               },
               {
                   xtype: 'displayfield',
                   fieldLabel: GIFTNAME,
                   id: 'product_name',
                   name: 'product_name',
                   allowBlank: false,
                   submitValue: true
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: GIFTNUM,
                   id: 'gift_amount',
                   name: 'gift_amount',
                   allowBlank: false,
                   allowDecimals: false,
                   submitValue: true,
                   value: 1,
                   minValue: 1
               },
                 {
                     xtype: 'textfield',
                     fieldLabel: GIFTNAME,
                     id: 'ticket_name',
                     name: 'ticket_name',
                     allowBlank: true,
                     submitValue: true,
                     hidden: true
                 },
               {
                   xtype: 'numberfield',
                   fieldLabel: BONUSWELFARE,
                   id: 'deduct_welfare',
                   name: 'deduct_welfare',
                   allowBlank: false,
                   allowDecimals:false,
                   submitValue: true,
                   minValue: 0,
                   hidden: true,
                   value: 0
               },
                 {
                     xtype: 'numberfield',
                     fieldLabel: MULRIPLE,
                     id: 'welfare_mulriple',
                     name: 'welfare_mulriple',
                     allowBlank: false,
                     allowDecimals: false,
                     submitValue: true,
                     minValue: 1,
                     hidden: true,
                     value: 1
                 },

               {
                   xtype: 'numberfield',
                   fieldLabel: FREIGHTPRICE,
                   id: 'freight_price',
                   name: 'freight_price',
                   allowBlank: false,
                   submitValue: true,
                   allowDecimals: false,
                   minValue: 0,
                   value: 0
               },
               {
                    xtype: 'numberfield',
                    fieldLabel: '購物金抵用券有效天數',
                    id: 'bonus_expire_day',
                    name: 'bonus_expire_day',
                    allowBlank: false,
                    submitValue: true,
                    allowDecimals: false,
                    hidden: true,
                    minValue: 0,
                    value: 30
               }
               ,
               {
                   xtype: 'numberfield',
                   fieldLabel: '使用間隔時間',
                   id: 'use_span_day',
                   name: 'use_span_day',
                   allowDecimals: false,
                   minValue: 0,
                   value: 15,
                   maxValue:9999999
               },
               {
                   xtype: "datetimefield",
                   fieldLabel: DATESTART,
                   editable: false,
                   id: 'tstart',
                   name: 'tstart',
                   anchor: '95%',
                   format: 'Y-m-d H:i:s',
                   width: 150,
                   allowBlank: false,
                   submitValue: true,
                   value: Tomorrow(),
                   time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                   listeners: {
                       select: function (a, b, c) {
                           var start = Ext.getCmp("tstart");
                           var end = Ext.getCmp("tend");
                           if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                               var start_date = start.getValue();
                               Ext.getCmp('tend').setValue(new Date(start_date.getFullYear(), start_date.getMonth()+ 1, start_date.getDate(),23,59,59));
                           }
                       }
                   }
               },
               {
                   xtype: "datetimefield",
                   fieldLabel: DATEEND,
                   editable: false,
                   id: 'tend',
                   anchor: '95%',
                   name: 'tend',
                   format: 'Y-m-d H:i:s',
                   allowBlank: false,
                   submitValue: true,
                   time: { hour: 23, min: 59, sec: 59 },
                   // value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                   value:setNextMonth(Tomorrow(),1),
                   listeners: {
                       select: function (a, b, c) {
                           var start = Ext.getCmp("tstart");
                           var end = Ext.getCmp("tend");
                           if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                               var end_date = end.getValue();
                               Ext.getCmp('tstart').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
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
                        if (Ext.getCmp('product_name').getValue() != "" && Ext.getCmp('product_name').getValue() != "沒有該商品信息！") {
                            form.submit({
                                params: {
                                    row_id: Ext.getCmp('trow_id').getValue(),
                                    this_event_id: Ext.getCmp('event_id').getValue(),
                                    group_id: Ext.getCmp('tgroup_id').getValue(),
                                    state1: Ext.getCmp('state1').getValue(),
                                    state2: Ext.getCmp('state2').getValue(),
                                    state3: Ext.getCmp('state3').getValue(),
                                    gift_id: Ext.getCmp('gift_id').getValue(),
                                    product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),
                                    ticket_name: Ext.htmlEncode(Ext.getCmp('ticket_name').getValue()),
                                    deduct_welfare: Ext.htmlEncode(Ext.getCmp('deduct_welfare').getValue()),
                                    welfare_mulriple: Ext.htmlEncode(Ext.getCmp('welfare_mulriple').getValue()),
                                    freight_price: Ext.htmlEncode(Ext.getCmp('freight_price').getValue()),
                                    bonus_expire_day: Ext.htmlEncode(Ext.getCmp('bonus_expire_day').getValue()),
                                    valid_end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('tend').getValue()), 'Y-m-d H:i:s')),
                                    valid_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('tstart').getValue()), 'Y-m-d H:i:s')),
                                    gift_amount: Ext.getCmp('gift_amount').getValue(),
                                    use_span_day: Ext.getCmp('use_span_day').getValue(),
                                    event_type: Ext.getCmp('event_type').getValue(),
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
                                row_id: Ext.getCmp('trow_id').getValue(),
                                this_event_id: Ext.getCmp('event_id').getValue(),
                                state1: Ext.getCmp('state1').getValue(),
                                state2: Ext.getCmp('state2').getValue(),
                                state3: Ext.getCmp('state3').getValue(),
                                gift_id: Ext.getCmp('gift_id').getValue(),
                                group_id: Ext.getCmp('tgroup_id').getValue(),
                                product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),
                                ticket_name: Ext.htmlEncode(Ext.getCmp('ticket_name').getValue()),
                                deduct_welfare: Ext.htmlEncode(Ext.getCmp('deduct_welfare').getValue()),
                                welfare_mulriple: Ext.htmlEncode(Ext.getCmp('welfare_mulriple').getValue()),
                                freight_price: Ext.htmlEncode(Ext.getCmp('freight_price').getValue()),
                                bonus_expire_day: Ext.htmlEncode(Ext.getCmp('bonus_expire_day').getValue()),
                                valid_end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('tend').getValue()), 'Y-m-d H:i:s')),
                                valid_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('tstart').getValue()), 'Y-m-d H:i:s')),
                                gift_amount: Ext.getCmp('gift_amount').getValue(),
                                event_type: Ext.getCmp('event_type').getValue(),
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
        height: 380,
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
                else {
                    p_editFrm.getForm().loadRecord(row); //如果是編輯的話
                    if (row.data.gift_type == 1) {
                        Ext.getCmp('state1').setValue(true);

                    }
                    else if (row.data.gift_type == 2) {
                        Ext.getCmp('state2').setValue(true);
                        Ext.getCmp('gift_amount').setValue(1);
                    }
                    else if (row.data.gift_type == 3) {
                        Ext.getCmp('state3').setValue(true);
                        Ext.getCmp('gift_amount').setValue(1);
                    }
                    Ext.getCmp('state1').setDisabled(true);
                    Ext.getCmp('state2').setDisabled(true);
                    Ext.getCmp('state3').setDisabled(true);
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
}
setNextMonth = function (source, n) {
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
