VipGroupStoreEdit.load();
function editFunction(row, store) {

    var currentPanel = 0;
    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {//首頁時進行第一步保存
                Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                var ffrom = firstForm.getForm();
                if (ffrom.isValid()) {
                                layout[direction]();
                                currentPanel++;
                                if (!layout.getNext()) {
                                    Ext.getCmp('move-next').hide();
                                }
                            else {
                        Ext.getCmp('move-next').setText(NEXT_MOVE);
                            }
                        }
                }
                    }
        else {
            layout[direction]();
            currentPanel--;
            if (currentPanel == 0) {
            Ext.getCmp('move-next').show();
                Ext.getCmp('move-prev').setDisabled(true); //第一次新增時上一步隱藏
        }
        }
    };
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", labelWidth: 80, msgTarget: "side" },
        items: [
                {
                  fieldLabel: 'row_id',
                  xtype: 'textfield',
                  id: 'row_id',
                  name: 'row_id',
                  hidden: true
              },
               {
                   fieldLabel: 'event_id',
                   xtype: 'textfield',
                   id: 'event_id',
                   name: 'event_id',
                   hidden: true
               },
            {
                    fieldLabel: EVENTNAME,
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                id: 'event_name',
                name: 'event_name',
                allowBlank: false
                },
                {
                    fieldLabel: EVENTDESC,
                    xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'event_desc',
                name: 'event_desc',
                allowBlank: false
                },
                {
                    fieldLabel: EVENTSTART,
                xtype: 'datetimefield',
                id: 'start',
                name: 'start',
                allowBlank: false,
                    editable: false,
                    format: 'Y-m-d H:i:s',
                    value: Tomorrow(),
                    time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                    listeners: {
                        select: function (a, b, c) {
                        var start = Ext.getCmp("start");
                        var end = Ext.getCmp("end");
                        if (end.getValue() < start.getValue()) {
                            var start_date = start.getValue();
                            Ext.getCmp('end').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(),23,59,59));
                            }
                        }
                    }
                },
                {
                    fieldLabel: EVENTEND,
                                   xtype: 'datetimefield',
                                   id: 'end',
                                   name: 'end',
                    format: 'Y-m-d H:i:s',
                    allowBlank: false,
                    editable: false,
                    time: { hour: 23, min: 59, sec: 59 },
                    value: setNextMonth(Tomorrow(), 1),
                    listeners: {
                        select: function (a, b, c) {
                                           var start = Ext.getCmp("start");
                                           var end = Ext.getCmp("end");
                                           if (end.getValue() < start.getValue()) {
                                               var end_date = end.getValue();
                                Ext.getCmp('start').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                            }
                        }
                    }

                               }
        ]
    });
    var secondForm = Ext.widget('form', {
        id: 'editFrm2',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: '/NewPromo/SaveQuestion',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'fieldcontainer',
                 layout: 'hbox',
                 items: [
                 {
                     xtype: 'textfield',
                     fieldLabel: "贈品編號",
                     allowBlank: false,
                     id: 'present_event_id',
                     name: 'present_event_id',
                     width: 273,
                     listeners: {
                         blur: function (text, o) {
                             var newValue = Ext.getCmp("present_event_id").getValue();
                             if (newValue != "") {//驗證贈品編號是否合法
                                 Ext.Ajax.request({
                                     url: '/NewPromo/GetNewPromoPresent',
                                     method: 'post',
                        params: {
                                         present_event_id: Ext.htmlEncode(Ext.getCmp("present_event_id").getValue())
                        },
                        success: function (form, action) {
                                         var result = Ext.decode(form.responseText);
                            if (result.success) {
                                             if (result.count == 0) {
                                                 Ext.Msg.alert(INFORMATION, "該贈品編號未啟用或不存在！");
                                                 Ext.getCmp("present_event_id").setValue("");
                                }
                                }
                            else {
                                             Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                                     failure: function () {
                                         Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }

            }

                                    }
                        },

                 {//商品設定
                     xtype: 'button',
                     text: "贈品新增",
                     id: 'btnProdId',
                     name: 'btnProdId',
                     margin: '0 0 0 9',
                     width: 70,
                     handler: function () {
                         var event_id_list = Ext.getCmp('present_event_id');
                         editPresentFunction(null, null, event_id_list, Ext.getCmp('start').getValue(), Ext.getCmp('end').getValue());
                                    }
                                }
                        ]
                    },
                    {
                xtype: 'combobox',
                fieldLabel: USERGROUP,
                id: 'group_id',
                name: 'group_id',
                store: VipGroupStoreEdit,
                displayField: 'group_name',
                valueField: 'group_id',
                        editable: false,
                lastQuery: '',
                value: "0"

                    },
                    {
                        xtype: 'radiogroup',
                fieldLabel: LIMITNEWUSER,
                id: 'new_user',
                name: 'new_user',
                        columns: 2,
                        defaults: {
                            flex: 1,
                    name: 'n_new_user'
                        },
                        items: [
                    {
                        boxLabel: YES, id: 'yes', inputValue: '1', checked: true
                    },
                    {
                        boxLabel: NO, id: 'no', inputValue: '0'
                            }
        ],
                                         listeners: {
                    change: function () {
                        var result = Ext.getCmp('yes').getValue();
                                                 if (result) {
                                                     Ext.getCmp('new_user_date').show();
                            Ext.getCmp('new_user_date').allowBlank = false;
                                         }
                        else {
                                                     Ext.getCmp('new_user_date').hide();
                                                     Ext.getCmp('new_user_date').allowBlank = true;
                                                 }
                                             }

                                     }
                     },
                     {
                         fieldLabel: USERTIME,
                         xtype: 'datetimefield',
                         id: 'new_user_date',
                         name: 'new_user_date',
                         allowBlank: false,
                         editable: false,
                         format: 'Y-m-d H:i:s',
                         time: { hour: 00, min: 00, sec: 00 },
                value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1))
                     },
                     {
                         fieldLabel: LIMITCONDI,
                         xtype: 'radiogroup',
                         id: 'count_by',
                         name: 'count_by',
                         columns: 2,
                         defaults: {
                             flex: 1,
                    name: 'c_count_by'
                         },
                         items: [
            { boxLabel: ORDER, id: 'order', inputValue: '1', checked: true },
                            { boxLabel: USERS, id: 'users', inputValue: '2' }
                         ]
                     },
                     {
                         fieldLabel: LIMITCOUNT,
                         xtype: 'numberfield',
                         id: 'count',
                         name: 'count',
                         minValue: 0,
                         maxValue: 100,
                         allowDecimals: false
                     },
                     {
                         fieldLabel: DEVICE,
                         xtype: 'radiogroup',
                         id: 'device',
                         name: 'device',
                         columns: 3,
                         defaults: {
                             flex: 1,
                    name: 'd_device'
                         },
                         items: [
                    {
                        boxLabel: NOSELECT, id: 'Any', inputValue: '0', checked: true
                    },
                   {
                       boxLabel: 'PC', id: 'PC', inputValue: '1'
                   },
                    {
                        boxLabel: PHONE, id: 'Phone_Pad', inputValue: '2'
                    }
                         ]
                     },

                     {
                         xtype: 'textfield',
                         fieldLabel: EVENTURL,
                         id: 'link_url',
                         name: 'link_url',
                allowBlank: true,
                vtype: 'url'
                     },
                     {
                         xtype: 'filefield',
                         fieldLabel: UPIMAGE,
                         id: 'promo_image',
                         name: 'promo_image',
                allowBlank: true,
                         buttonText: UPLOAD
                     },
                     {
                         fieldLabel: ACTIVENEW,
                         xtype: 'radiogroup',
                         id: 'active_now',
                         name: 'active_now',
                         columns: 2,
                         defaults: {
                             flex: 1,
                    name: 'a_active_now'
                         },
                         items: [
                    {
                        boxLabel: YES, id: 'today', inputValue: '1', checked: true
                    },
                   {
                       boxLabel: NO, id: 'notoday', inputValue: '0'
                     }
        ]
            },
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
              
                var boolReturn = true;
                if (Ext.getCmp('present_event_id').getValue() != "") {
                    Ext.Ajax.request({
                        url: '/NewPromo/GetNewPromoPresent',
                        method: 'post',
                        async: false,
                        params: {
                            present_event_id: Ext.htmlEncode(Ext.getCmp("present_event_id").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                if (result.count == 0) {
                                    Ext.Msg.alert(INFORMATION, "該贈品編號未啟用或不存在！");
                                    Ext.getCmp("present_event_id").setValue("");
                                    boolReturn = false;
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
                if (form.isValid() && boolReturn) {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                myMask.show();
                    form.submit({
                        params: {
                            row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                            event_name: Ext.htmlEncode(Ext.getCmp('event_name').getValue()),
                            event_desc: Ext.htmlEncode(Ext.getCmp('event_desc').getValue()),
                            start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start').getValue()), 'Y-m-d H:i:s')),
                            end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end').getValue()), 'Y-m-d H:i:s')),
                            event_id: Ext.htmlEncode(Ext.getCmp('event_id').getValue()),
                            present_event_id: Ext.htmlEncode(Ext.getCmp('present_event_id').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            new_user: Ext.htmlEncode(Ext.getCmp('new_user').getValue().n_new_user),
                            new_user_date: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('new_user_date').getValue()), 'Y-m-d H:i:s')),//會員註冊日期
                            count_by: Ext.htmlEncode(Ext.getCmp('count_by').getValue().c_count_by),
                            count: Ext.htmlEncode(Ext.getCmp('count').getValue()),
                            device: Ext.htmlEncode(Ext.getCmp('device').getValue().d_device),
                            link_url: Ext.htmlEncode(Ext.getCmp('link_url').getValue()),
                            promo_image: Ext.htmlEncode(Ext.getCmp('promo_image').getValue()),
                            active_now: Ext.htmlEncode(Ext.getCmp('active_now').getValue().a_active_now)
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                myMask.hide();
                                if (result.msg != undefined) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                } else {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    editWin.close();
                                    store.load();
                                }
                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);

                        }
                    });
                }
            }
        }]
    });
    var allpan = new Ext.panel.Panel({
        width: 390,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            // 应用到所有子面板
            border: false
        },
        // 这里仅仅用几个按钮来示例一种可能的导航场景.
        bbar: [
        {
            id: 'move-prev',
            text: PREV,
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            },
            disabled: true
        },
        '->', // 一个长间隔, 使两个按钮分布在两边
        {
            id: 'move-next',
            text: NEXT,
            handler: function (btn) {
                navigate(btn.up("panel"), "next");
            }
        }
        ],
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "問卷送禮",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [allpan],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: CLOSE,
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             editWin.destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                if (row) {
                    firstForm.getForm().loadRecord(row);
                    secondForm.getForm().loadRecord(row);
                    initForm(row);
                }
                else {
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initForm(Row) {

        if (!Row.data.new_user) {
            Ext.getCmp('no').setValue(true);
        }
        else {
            Ext.getCmp('yes').setValue(true);
        }
        if (Row.data.count_by == 1) {
            Ext.getCmp('order').setValue(true);
        }
        else {
            Ext.getCmp('users').setValue(true);
        }
        if (Row.data.device == 0) {
            Ext.getCmp('Any').setValue(true);
        }
        else if (Row.data.device == 1) {
            Ext.getCmp('PC').setValue(true);
        }
        else if (Row.data.device == 2) {
            Ext.getCmp('Phone_Pad').setValue(true);
        }
        if (!Row.data.active_now) {
            Ext.getCmp('notoday').setValue(true);
        }
        else {
            Ext.getCmp('today').setValue(true);
        }

        if (Row.data.promo_image != "") {
            Ext.getCmp('promo_image').setRawValue(Row.data.promo_image);
    }
}
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
//setNextMonth = function (source, n) {
//    var s = new Date(source);
//    s.setMonth(s.getMonth() + n);
//    if (n < 0) {
//        s.setHours(0, 0, 0);
//    }
//    else if (n > 0) {
//        s.setHours(23, 59, 59);
//    }
//    return s;
    //}
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


