var code = "";
var levelitems = [];
//動態獲得會員級別數據
Ext.Ajax.request({
    url: '/MemberEvent/GetMemberLevelDownList',
    method: 'POST',
    //async: false,
    success: function (response) {
        var res = Ext.JSON.decode(response.responseText);
        var data = res.data;
        for (var i = 0; i < data.length; i++) {
            levelitems.push({
                boxLabel: data[i].ml_name,
                name: 'level',
                inputValue: data[i].ml_code
            });
        }

    }
});
editMemberEventFunction = function (row, store) {
    if (row != null && row.data.ml_code.toString() != "0") {
        code = row.data.ml_code;
    }
    var event_type = 1;
    var tab = Ext.create('Ext.tab.Panel', {
        height: 90,
        minTabWidth: 140,
        renderTo: document.body,
        items: [{
            id: 'everyday',
            title: '每日',
            listeners:
                {
                    activate: function (e) {
                        event_type = 1;
                    }
                }
        }, {
            id: 'everyweek',
            title: '每週',
            items: [
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    width: 500,
                    margin: '10 0 0 10',
                    items: [
                       {
                           xtype: 'checkboxgroup',
                           fieldLabel: '每週',
                           columns: 4,
                           id: 'wk',
                           name: 'wk',
                           width: 300,
                           vertical: false,
                           items: [
                               { boxLabel: '一', name: 'week', inputValue: '1' },
                               { boxLabel: '二', name: 'week', inputValue: '2' },
                               { boxLabel: '三', name: 'week', inputValue: '3' },
                               { boxLabel: '四', name: 'week', inputValue: '4' },
                               { boxLabel: '五', name: 'week', inputValue: '5' },
                               { boxLabel: '六', name: 'week', inputValue: '6' },
                               { boxLabel: '日', name: 'week', inputValue: '7' }
                           ]
                       }
                    ]
                }
            ],
            listeners:
                {
                    activate: function (e) {
                        event_type = 2;
                    }
                }
        },
        {
            id: 'everymonth',
            title: '每月',
            items: [
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    width: 500,
                    margin: '10 0 0 0',
                    items: [
                        {
                            xtype: 'combobox',
                            id: 'month',
                            name: 'month',
                            margin: '0 5px',
                            fieldLabel: '活動日期',
                            colName: 'day',
                            queryMode: 'local',
                            editable: false,
                            allowBlank: false,
                            store: dayStore,
                            displayField: 'txt',
                            valueField: 'value',
                            value: 1,
                            listeners: {
                                select: function (records, e) {
                                    var month30 = [2, 4, 6, 9, 11];
                                    var message = '';
                                    if (parseInt(records.value) > 28) {
                                        var starttime = Ext.getCmp('me_startdate').getValue();
                                        var endtime = Ext.getCmp('me_enddate').getValue();
                                        starttime = new Date(starttime);
                                        endtime = new Date(endtime);
                                        var syear = starttime.getFullYear();
                                        var smonth = starttime.getMonth() + 1;
                                        var eyear = endtime.getFullYear();
                                        var emonth = endtime.getMonth() + 1;
                                        //同一年
                                        if (syear == eyear) {
                                            if (parseInt(records.value) == 31) {
                                                if (smonth == emonth) {
                                                    if (Ext.Array.indexOf(month30, smonth, 0) != -1) {
                                                        message += smonth + "月沒有31日;";
                                                    }
                                                }
                                                for (var i = smonth; i < emonth; i++) {
                                                    if (Ext.Array.indexOf(month30, i, 0) != -1) {
                                                        message += i + "月沒有31日;";
                                                    }
                                                }
                                            }
                                            //判斷是否包含2月份
                                            if (smonth <= 2 && emonth >= 2) {
                                                if (parseInt(records.value) == 30) {
                                                    message += "2月沒有30日;";
                                                }
                                                if (parseInt(records.value) == 29 && syear % 4 != 0) {
                                                    message += "2月沒有29日;";
                                                }
                                            }
                                        }
                                        else {
                                            if (parseInt(records.value) == 31) {
                                                for (var i = smonth; i <= 12; i++) {
                                                    if (Ext.Array.indexOf(month30, i, 0) != -1) {
                                                        message += syear + "年" + i + "月沒有31日;";
                                                    }
                                                }
                                                if (eyear - syear > 1) {
                                                    for (var y = syear + 1; y < eyear; y++) {
                                                        message += y + "年" + "2月, 4月, 6月, 9月, 11月沒有31日;";

                                                    }
                                                }
                                                for (var i = 1; i < emonth; i++) {
                                                    if (Ext.Array.indexOf(month30, i, 0) != -1) {
                                                        message += eyear + "年" + i + "月沒有31日;";
                                                    }
                                                }

                                            }
                                            //判斷開始日期是否包含2月份
                                            if (smonth <= 2) {
                                                if (parseInt(records.value) == 30) {
                                                    message += syear + "年" + "2月沒有30日;";
                                                }
                                                if (parseInt(records.value) == 29 && syear % 4 != 0) {
                                                    message += syear + "年" + "2月沒有29日;";
                                                }
                                            }
                                            if (eyear - syear > 1) {
                                                for (var y = syear + 1; y < eyear; y++) {
                                                    if (parseInt(records.value) == 30) {
                                                        message += y + "年" + "2月沒有30日;";
                                                    }
                                                    if (parseInt(records.value) == 29 && y % 4 != 0) {
                                                        message += y + "年" + "2月沒有29日;";
                                                    }
                                                }
                                                //
                                            }
                                            //判斷結束日期是否包含2月份
                                            if (emonth >= 2) {
                                                if (parseInt(records.value) == 30) {
                                                    message += eyear + "年" + "2月沒有30日;";
                                                }
                                                if (parseInt(records.value) == 29 && syear % 4 != 0) {
                                                    message += eyear + "年" + "2月沒有29日;";
                                                }
                                            }


                                        }
                                    }
                                    if (message != '') {
                                        Ext.Msg.alert(INFORMATION, message);
                                    }
                                }
                            }
                        },
                        {
                            xtype: 'displayfield',
                            value: '日'
                        }
                    ]
                }
            ],
            listeners:
               {
                   activate: function (e) {
                       event_type = 3;
                   }
               }
        }
        ]
    });
    var editMemberEventFrm = Ext.create('Ext.form.Panel', {
        id: 'editMemberEventFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 60,
        url: '/MemberEvent/SaveMemberEvent',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '流水號',
                id: 'rowID',
                name: 'rowID',
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '活動名稱',
                id: 'me_name',
                name: 'me_name',
                submitValue: true,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '活動描述',
                id: 'me_desc',
                name: 'me_desc',
                submitValue: true,
                allowBlank: false
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 500,
                items: [
                       {
                           fieldLabel: "活動期間",
                           xtype: 'datefield',
                           id: 'me_startdate',
                           name: 'me_startdate',
                           allowBlank: false,
                           editable: false,
                           format: 'Y-m-d',
                           value: Today(),
                           //value:new Date(),
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("me_startdate");
                                   var end = Ext.getCmp("me_enddate");

                                   var s_date = new Date(start.getValue());
                                   end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                               }
                           }
                       },
                       {
                           xtype: 'displayfield',
                           margin: '0 5 0 5',
                           value: '~ '
                       },
                       {
                           xtype: 'datefield',
                           id: 'me_enddate',
                           name: 'me_enddate',
                           format: 'Y-m-d',
                           value: new Date(Today().setMonth(Today().getMonth() + 1)),
                           allowBlank: false,
                           editable: false,
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("me_startdate");
                                   var end = Ext.getCmp("me_enddate");
                                   var s_date = new Date(start.getValue());
                                   if (end.getValue() < start.getValue()) {
                                       Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                       end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                   }
                               }
                           }

                       }
                ]
            },
            tab,
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 500,
                items: [
                      {
                          xtype: 'displayfield',
                          fieldLabel: '活動時間'
                      },
                      {
                          xtype: 'timefield',
                          id: 'et_starttime',
                          name: 'et_starttime',
                          format: 'H:i:s',
                          altFormats: 'H:i:s',
                          autoScroll: true,
                          allowBlank: false,
                          regex: /^([0-1]?[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$/,
                          //regexText: '請輸入正確的時間',
                          increment: 30,
                          forceSelection: true,
                          value: '00:00:00',
                          validator: function (value) {
                              var end = Ext.getCmp('et_endtime').getValue();
                              var start = Ext.getCmp('et_starttime').getValue();
                              if (start > end) {
                                  return '開始時間不能大於結束時間!';
                              }
                              else {
                                  return true;
                              }
                          }
                      },
                     {
                         xtype: 'displayfield',
                         value: '至',
                         labelWidth: 25,
                         margin: '0 5 0 5'
                     },
                     {
                         xtype: 'timefield',
                         id: 'et_endtime',
                         name: 'et_endtime',
                         format: 'H:i:s',
                         altFormats: 'H:i:s',
                         autoScroll: true,
                         allowBlank: false,
                         regex: /^([0-1]?[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$/,
                         //regexText :'請輸入正確的時間',
                         increment: 30,
                         value: '23:59:59',
                         validator: function (value) {
                             var end = Ext.getCmp('et_endtime').getValue();
                             var start = Ext.getCmp('et_starttime').getValue();
                             if (end < start) {
                                 return '結束時間不能小於開始時間!';
                             }
                             else {
                                 return true;
                             }
                         }
                     }]
            },
            {
                xtype: 'fieldcontainer',
                //fieldLabel: '壽星會員獨享',
                defaultType: 'checkboxfield',
                id: 'me_birthday',
                name: 'me_birthday',
                items: [
                    {
                        boxLabel: '壽星會員獨享',
                        name: 'me_birthday',
                        inputValue: '1'
                        //id: 'me_birthday'
                    }
                ]
            },
            {
                xtype: 'textfield',
                fieldLabel: '促銷編號',
                id: 'event_id',
                name: 'event_id',
                submitValue: true,
                allowBlank: false,
                listeners: {
                    blur: function () {
                        var event_id = Ext.getCmp('event_id').getValue();
                        Ext.Ajax.request({
                            url: '/MemberEvent/IsGetEventID',
                            params: {
                                event_id: event_id,
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    var data = "促銷期間: " + result.time1 + "~" + result.time2;
                                }
                                else {
                                    var data = "促銷編號錯誤或促銷活動未啟用";
                                    Ext.getCmp('event_id').setValue('');
                                }
                                Ext.getCmp('event_id_info').setValue(data);
                            }
                        });
                    }
                }
            },
            {
                xtype: 'displayfield',
                value: '',
                margin: '0 0 0 105',
                id:'event_id_info',
            },
            {
                xtype: 'filefield',
                id: 'me_big_banner',
                name: 'me_big_banner',
                fieldLabel: '活動圖',
                buttonText: '選擇檔案',
                allowBlank: false,
                validator:
function (value) {
    if (value != '') {
        var type = value.split('.');
        var extention = type[type.length - 1].toString().toLowerCase();
        if (extention == 'gif' || extention == 'png' || extention == 'jpg' || extention == 'GIF' || extention == 'PNG' || extention == 'JPG') {
            return true;
        }
        else {
            return '上傳文件類型不正確！';
        }
    }
    else {
        return true;
    }
},

            },
            {
                xtype: 'textfield',
                id:'me_banner_link',
                name: 'me_banner_link',
                fieldLabel: '連接地址',
                vtype: 'url',
                allowBlank:false,

            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                width: 500,
                items: [
                    {
                        xtype: 'displayfield',
                        value: '是否限領一次'
                    },
                    {
                        xtype: 'radiogroup',
                        allowBlank: false,
                        columns: 2,
                        width: 250,
                        id: 'me_bonus_onetime',
                        name: 'me_bonus_onetime',
                        margin: '0 0 0 20',
                        items: [
                        {
                            boxLabel: '否',
                            name: 'me_bonus_onetime',
                            id: 'n',
                            inputValue: 0,
                            checked: true
                        },
                        {
                            boxLabel: '是',
                            name: 'me_bonus_onetime',
                            id: 'y',
                            inputValue: 1
                        }]
                    }

                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                width: 500,
                items: [
                    {
                        xtype: 'displayfield',
                        value: '活動會員級別'
                    },
                    {
                        xtype: 'radiogroup',
                        allowBlank: false,
                        columns: 2,
                        id: 'ml_code',
                        name: 'ml_code',
                        width: 250,
                        margin: '0 0 0 20',
                        items: [{
                            boxLabel: '不分',
                            name: 'ml_code',
                            id: 'bufen',
                            inputValue: '0',
                            checked: true,
                            listeners:
                            {
                                change: function (newValue) {
                                    if (newValue) {
                                        Ext.getCmp('btnlevel').setDisabled(false);
                                    }
                                }

                            }
                        },
                        {
                            boxLabel: '會員級別選定',
                            name: 'ml_code',
                            id: 'fen',
                            inputValue: '',
                            listeners:
                             {
                                 change: function (newValue) {
                                     if (newValue) {
                                         Ext.getCmp('btnlevel').setDisabled(true);
                                     }
                                 }

                             }
                        }]
                    },
                     {
                         xtype: 'button',
                         id: 'btnlevel',
                         text: '選擇級別',
                         //width:120,
                         disabled: true,
                         handler: function () {
                             showMemberLevelForm(row);
                         }
                     }
                ]
            }],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            type: 'submit',
            handler: function () {
                var form = this.up('form').getForm();
                var wk = Ext.getCmp('wk').getChecked();
                var wek = "";
                //獲得選擇的值
                Ext.Array.each(wk, function (item) {
                    wek += item.inputValue + ',';
                });
                var fen = Ext.getCmp('fen').getValue();
                var ok = true;
                var message = "";
                //如果選擇的是不分，就清空會員等級
                if (fen == false) {
                    code = '';
                }
                if (fen == true && code == '') {
                    ok = false;
                    message = '請選擇會員級別!';
                }
                if (event_type == 2 && wek == "") {
                    ok = false;
                    message = '請選擇活動日!';
                }
                if (form.isValid()) {
                    if (ok) {
                        form.submit({
                            params: {
                                rowID: Ext.htmlEncode(Ext.getCmp('rowID').getValue()),
                                me_name: Ext.htmlEncode(Ext.getCmp('me_name').getValue()),
                                me_desc: Ext.htmlEncode(Ext.getCmp('me_desc').getValue()),
                                me_startdate: Ext.Date.format(new Date(Ext.getCmp('me_startdate').getValue()), 'Y-m-d'),
                                me_enddate: Ext.Date.format(new Date(Ext.getCmp('me_enddate').getValue()), 'Y-m-d'),
                                et_id: row == null ? 0 : row.data.et_id,
                                me_birthday: Ext.getCmp('me_birthday').items.items[0].value,
                                event_id: Ext.htmlEncode(Ext.getCmp('event_id').getValue()),
                                me_big_banner: Ext.getCmp('me_big_banner').getValue(),
                                me_bonus_onetime: Ext.htmlEncode(Ext.getCmp('me_bonus_onetime').getValue()),
                                code: code,
                                et_name: event_type,
                                week: wek,
                                month: Ext.htmlEncode(Ext.getCmp('month').getValue()),
                                et_starttime: Ext.htmlEncode(Ext.getCmp('et_starttime').getValue()),
                                et_endtime: Ext.htmlEncode(Ext.getCmp('et_endtime').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.JSON.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    store.load();
                                    Ext.getCmp('editMemberEventWin').destroy();

                                }
                                else {
                                    if (result.msg == "促銷編號錯誤或促銷活動未啟用") {
                                        Ext.getCmp('event_id').setValue('');
                                    }
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.JSON.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        });
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, message);
                    }
                }
            }
        }]
    });
    var editMemberEventWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editMemberEventWin',
        width: 545,
        layout: 'anchor',
        items: [editMemberEventFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: row == null ? '會員活動新增' : '會員活動編輯',
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
                         Ext.getCmp('editMemberEventWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            resize: function () {
                h = Ext.getCmp('editMemberEventFrm').getHeight() + 50;
                Ext.getCmp('editMemberEventWin').setHeight(h);
                this.doLayout();
            },
            'show': function () {

                if (row != null) {
                    Ext.getCmp('me_big_banner').setRawValue(row.data.me_big_banner);
                    editMemberEventFrm.getForm().loadRecord(row);
                    if (row.data.ml_code.toString() == "0") {
                        Ext.getCmp('bufen').setValue(true);
                    }
                    else {
                        Ext.getCmp('fen').setValue(true);
                    }
                    switch (row.data.et_name) {
                        case "DD":
                            tab.setActiveTab(0);
                            break;
                        case "WW":
                            tab.setActiveTab(1);
                            Ext.getCmp('wk').setValue({
                                week: row.data.et_date_parameter.toString().split(",")
                            });
                            break;
                        case "MM":
                            tab.setActiveTab(2);
                            Ext.getCmp('month').setValue(row.data.et_date_parameter);
                            break;
                    }
                    if (row.data.me_bonus_onetime == "0") {
                        Ext.getCmp('n').setValue(true);
                    }
                    else {
                        Ext.getCmp('y').setValue(true);
                    }
                }

            }
        }
    });
    editMemberEventWin.show();
}
function Today() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}