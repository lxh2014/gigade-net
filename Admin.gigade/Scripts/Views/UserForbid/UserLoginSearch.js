//查询
Query = function ()
{
    UserLoginStore.removeAll();
    var sismail = Ext.getCmp('ismail').getValue();
    switch (sismail.ismail)
    {
        case 0:
            Ext.getCmp('mail').show();
            Ext.getCmp('ipfrom').show();
            break;
        case 1:
            Ext.getCmp('mail').show();
            Ext.getCmp('ipfrom').hide();
            break;
        case 2:
            Ext.getCmp('mail').hide();
            Ext.getCmp('ipfrom').show();
            break;

    }
    Ext.getCmp("gdUserLogin").store.loadPage(1, {
        params: {
            //user_id: Ext.getCmp('user_id').getValue(),
            login_mail: Ext.getCmp('login_mail').getValue(),
            login_ipfrom: Ext.getCmp('login_ipfrom').getValue(),
            start_date: Ext.getCmp('start').getValue(),
            end: Ext.getCmp('end').getValue(),
            sumtotal: Ext.getCmp('sumtotal').getValue(),
            ismail: Ext.getCmp('ismail').getValue(),
            login_type: Ext.getCmp('login_type').getValue()
        }
    });
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
    return dt;                  // 返回日期。
}
function setNextMonth(source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n >= 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
//登入錯誤類型
var LoginTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有類型', "value": "0" },
        { "txt": '會員登入錯誤', "value": "1" },
        { "txt": 'PHP後台登入錯誤', "value": "2" },
        { "txt": 'NET後台登入錯誤', "value": "3" },
        { "txt": '查詢機密資料登入錯誤', "value": "4" }
    ]
});
var frm = Ext.create('Ext.form.Panel', {
    id: 'frm',
    layout: 'anchor',
    flex: 2,
    border: 0,
    bodyPadding: 10,
    width: document.documentElement.clientWidth,
    items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'textfield',
                    id: 'login_mail',
                    name: 'login_mail',
                    margin: '0 5px',
                    fieldLabel: '用戶郵箱',
                    labelWidth: 70,
                    editable: false,
                    listeners: {
                        specialkey: function (field, e)
                        {
                            // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                            // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                            if (e.getKey() == e.ENTER)
                            {
                                Query();
                            }
                        }
                    }
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'textfield',
                    id: 'login_ipfrom',
                    name: 'login_ipfrom',
                    margin: '0 5px',
                    fieldLabel: '來源IP',
                    labelWidth: 70,
                    listeners: {
                        specialkey: function (field, e)
                        {
                            if (e.getKey() == e.ENTER)
                            {
                                Query();
                            }
                        }
                    }

                },
                {
                    xtype: 'textfield',
                    id: 'sumtotal',
                    name: 'sumtotal',
                    margin: '0 5px 0 20px',
                    fieldLabel: '登入失敗次數≥',
                    regex: /^\d+$/,
                    regexText: '請輸入數字',
                    listeners: {
                        specialkey: function (field, e)
                        {
                            // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                            // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                            if (e.getKey() == e.ENTER)
                            {
                                Query();
                            }
                        }
                    }
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'displayfield',
                    margin: '0 5px',
                    fieldLabel: '日期區間',
                    labelWidth: 60
                },
                {
                    xtype: 'datetimefield',
                    id: 'start',
                    name: 'start',
                    margin: '0 5px 0 0',
                    width: 150,
                    format: 'Y-m-d H:i:s',
                    time: { hour: 00, min: 00, sec: 00 },
                    editable: false,
                    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                    listeners: {
                        select: function ()
                        {
                            var start = Ext.getCmp('start');
                            var endDate = Ext.getCmp('end');
                            if (endDate.getValue() == null) {
                                endDate.setValue(setNextMonth(start.getValue(), 1));
                            }
                            else if (start.getValue() > endDate.getValue()) {
                                endDate.setValue(setNextMonth(start.getValue(), 1));
                            }
                        }
                    }
                },
                {
                    xtype: 'displayfield',
                    value: '~',
                    margin: '0 5px'
                },
                {
                    xtype: 'datetimefield',
                    id: 'end',
                    name: 'end',
                    margin: '0 5px',
                    width: 150,
                    format: 'Y-m-d  H:i:s',
                    time: { hour: 23, min: 59, sec: 59 },
                    editable: false,
                    value: setNextMonth(Tomorrow(), 0),
                    listeners: {
                        select: function ()
                        {
                            var start = Ext.getCmp('start');
                            var endDate = Ext.getCmp('end');
                            if (start.getValue() == null) {
                                start.setValue(setNextMonth(endDate.getValue(), -1));
                            }
                            else if (endDate.getValue() < start.getValue()) {
                                start.setValue(setNextMonth(endDate.getValue(), -1));
                            }
                        }
                    }
                }
            ]
        },
         {
             xtype: 'fieldcontainer',
             combineErrors: true,
             layout: 'hbox',
             //defaults: {
             //    flex: 0.5
             //},
             width: 700,
             items: [
                 {
                     xtype: 'displayfield',
                     margin: '0 5px',
                     //value: '統計',
                     fieldLabel: '統計條件',
                     labelWidth: 60
                 },
                 {
                     xtype: 'radiogroup',
                     id: 'ismail',
                     name: 'ismail',
                     colName: 'ismail',
                     allowBlank: false,
                     columns: 3,
                     width: 300,
                     margin: '0 0 0 20',
                     items: [{
                         boxLabel: '郵箱&IP',
                         name: 'ismail',
                         id: 'alls',
                         inputValue: 0,
                         checked: true
                         //,
                         //listeners: {
                         //    change: function (radio, oldValue, newValue, e) {
                         //        if (newValue) {
                         //            var nv = Ext.getCmp('ismail').getValue();
                         //            switch (nv.ismail) {
                         //                case 1:
                         //                    Ext.getCmp('ipfrom').hide();
                         //                    Ext.getCmp('mail').show();
                         //                    break;
                         //                case 2:
                         //                    Ext.getCmp('mail').hide();
                         //                    Ext.getCmp('ipfrom').show();
                         //                    break;
                         //                default:
                         //                    Ext.getCmp('ipfrom').show();
                         //                    Ext.getCmp('mail').show();
                         //                    break;
                         //            }

                         //        }

                         //    }
                         //}

                     }, {
                         boxLabel: '登入郵箱',
                         name: 'ismail',
                         id: 'ygp',
                         inputValue: 1
                         //,
                         //listeners: {
                         //    change: function (radio, oldValue, newValue, e) {
                         //        if (newValue) {
                         //            var nvv = Ext.getCmp('ismail').getValue();
                         //            switch (nvv.ismail) {
                         //                case 2:
                         //                    Ext.getCmp('mail').hide();
                         //                    Ext.getCmp('ipfrom').show();
                         //                    break;
                         //                default:
                         //                    Ext.getCmp('ipfrom').show();
                         //                    Ext.getCmp('mail').show();
                         //                    break;
                         //            }
                         //        }
                         //    }
                         //}
                     },
                     {
                         boxLabel: '來源IP',
                         name: 'ismail',
                         id: 'ngp',
                         inputValue: 2
                         //,
                         //listeners: {
                         //    change: function (radio, oldValue, newValue, e) {
                         //        if (newValue) {
                         //            var v = Ext.getCmp('ismail').getValue();
                         //            switch (v.ismail) {
                         //                case 1:
                         //                    Ext.getCmp('ipfrom').hide();
                         //                    Ext.getCmp('mail').show();
                         //                    break;
                         //                default:
                         //                    Ext.getCmp('ipfrom').show();
                         //                    Ext.getCmp('mail').show();
                         //                    break;
                         //            }
                         //        }
                         //    }
                         //}

                     }]
                 },
                 {
                     xtype: 'combobox',
                     id: 'login_type',
                     name: 'login_type',
                     fieldLabel: '登入錯誤類型',
                     queryMode: 'local',
                     store: LoginTypeStore,
                     displayField: 'txt',
                     valueField: 'value',
                     value: 0,
                     typeAhead: true,
                     editable: false,
                     hiddenName: 'value'
                 }

             ]
         },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function ()
                            {
                                Ext.getCmp('login_mail').setValue("");
                                Ext.getCmp('login_ipfrom').setValue("");
                                Ext.getCmp('login_type').setValue(0);
                                Ext.getCmp('sumtotal').setValue("");
                                Ext.getCmp('start').reset();
                                Ext.getCmp('end').reset();
                                Ext.getCmp('alls').setValue(true);
                                Query();
                            }
                        }
                    }
                ]
            }
    ]
});
