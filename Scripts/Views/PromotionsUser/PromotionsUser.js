var condition_id = "";
var ShowUserConInfo = "";

function loadData(conditionID) {
    if (conditionID != null && conditionID != "") {
        Ext.Ajax.request({
            url: '/PromotionsUser/UserConInfo',
            method: 'post',
            params: {
                condition_id: conditionID
            },
            success: function (form, action) {
                var resText = Ext.decode(form.responseText);
                if (resText.data[0].reg_start != "0") {
                    var reg_start = new Date(resText.data[0].reg_start * 1000);
                    Ext.getCmp("reg_start").setValue(reg_start);
                }
                if (resText.data[0].reg_end != "0") {
                    var reg_end = new Date(resText.data[0].reg_end * 1000);
                    Ext.getCmp("reg_end").setValue(reg_end);
                }

                Ext.getCmp("reg_interval").setValue(resText.data[0].reg_interval);
                Ext.getCmp("buy_times_min").setValue(resText.data[0].buy_times_min);
                Ext.getCmp("buy_times_max").setValue(resText.data[0].buy_times_max);
                Ext.getCmp("buy_amount_min").setValue(resText.data[0].buy_amount_min);
                Ext.getCmp("buy_amount_max").setValue(resText.data[0].buy_amount_max);

                if (resText.data[0].reg_start != "0") {
                    var last_time_start = new Date(resText.data[0].last_time_start * 1000);
                    Ext.getCmp("last_time_start").setValue(last_time_start);
                }
                if (resText.data[0].last_time_end != "0") {
                    var last_time_end = new Date(resText.data[0].last_time_end * 1000);
                    Ext.getCmp("last_time_end").setValue(last_time_end);
                }
                Ext.getCmp("last_time_interval").setValue(resText.data[0].last_time_interval);
                Ext.getCmp("join_channel").setValue(resText.data[0].join_channel);
            }
        })
    }
}




showUserSetForm = function (condition_name, conditionID, TxtId) {

    condition_id = conditionID;

    //    //會員來源
    //    Ext.define("gigade.UserSource", {
    //        extend: 'Ext.data.Model',
    //        fields: [
    //        { name: "id", type: "string" },
    //        { name: "name", type: "string"}]
    //    });

    //    //會員來源store
    //    var UserSourceStore = Ext.create('Ext.data.Store', {
    //        model: 'gigade.UserSource',
    //        data: [
    //        { "id": "AL", "name": "Alabama" },
    //        { "id": "AK", "name": "Alaska" },
    //        { "id": "AZ", "name": "Arizona" }
    //    ]
    //    });

    var form = Ext.widget('form', {
        border: false,
        frame: true,
        bodyPadding: 10,
        layout: 'anchor',
        url: '/PromotionsUser/SavePromotionsUser',
        fieldDefaults: {
            labelAlign: 'left',
            labelWidth: 120,
            anchor: '100%'
        },
        items: [
                 {
                     xtype: "datetimefield",
                     fieldLabel: USERSTART,
                     id: 'reg_start',
                     name: 'reg_start',
                     format: 'Y-m-d H:i:s',
                     allowBlank: false,
                     editable: false,
                     submitValue: true,
                     value: Tomorrow(),
                     listeners: {
                         select: function (a, b, c) {
                             var start = Ext.getCmp("reg_start");
                             var end = Ext.getCmp("reg_end");
                             var s_date = new Date(start.getValue());
                             end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                         }
                     }
                 },
                {
                    xtype: "datetimefield",
                    fieldLabel: USEREND,
                    format: 'Y-m-d H:i:s',
                    id: 'reg_end',
                    editable: false,
                    name: 'reg_end',
                    allowBlank: false,
                    submitValue: true,
                    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("reg_start");
                            var end = Ext.getCmp("reg_end");
                            var s_date = new Date(start.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, TIMETIP);
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        }
                    }
                },
                 {
                     xtype: 'fieldcontainer',
                     layout: 'hbox',
                     defaults: {
                         labelWidth: 120,
                         width: 180
                     },
                     items: [
                          {
                              xtype: "numberfield",
                              fieldLabel: USERTIME,
                              id: 'reg_interval',
                              name: 'reg_interval',
                              allowBlank: false,
                              minValue: 0
                          },
                         {
                             xtype: 'displayfield',
                             value: USERTIMEMONTH
                         }
                     ]
                 },

                 {
                     xtype: 'displayfield',
                     value: USERTIMEMONTHINFO
                 },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: CONSUMENUMBER,
                    defaults: {
                        labelWidth: 30,
                        width: 95,
                        margin: '0 5 0 0'
                    },

                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                        {
                            xtype: "numberfield",
                            fieldLabel: CONSUMENUMBERMIN,
                            id: 'buy_times_min',
                            name: 'buy_times_min',
                            allowBlank: false,
                            minValue: 0
                        },
                        {
                            xtype: 'displayfield',
                            value: RANGE,
                            width: 20
                        },
                        {
                            xtype: "numberfield",
                            fieldLabel: CONSUMENUMBERMAX,
                            id: 'buy_times_max',
                            name: 'buy_times_max',
                            allowBlank: false,
                            minValue: 0
                        }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: CONSUMEMONEY,
                    layout: 'hbox',
                    defaults: {
                        labelWidth: 30,
                        width: 95,
                        margin: '0 5 0 0'
                    },
                    items: [
                         {
                             xtype: "numberfield",
                             fieldLabel: CONSUMEMONEYMIN,
                             id: 'buy_amount_min',
                             allowBlank: false,
                             name: 'buy_amount_min',
                             minValue: 0
                         },
                        {
                            xtype: 'displayfield',
                            value: RANGE,
                            width: 20
                        },
                        {
                            xtype: "numberfield",
                            fieldLabel: CONSUMEMONEYMAX,
                            id: 'buy_amount_max',
                            name: 'buy_amount_max',
                            allowBlank: false,
                            minValue: 0
                        }
                    ]
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: LASTCONSUMETIMESTART,
                    id: 'last_time_start',
                    name: 'last_time_start',
                    format: 'Y-m-d H:i:s',
                    editable: false,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("last_time_start");
                            var end = Ext.getCmp("last_time_end");
                            var s_date = new Date(start.getValue());
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    }
                }, {
                    xtype: "datetimefield",
                    fieldLabel: LASTCONSUMETIMEEND,
                    format: 'Y-m-d H:i:s',
                    id: 'last_time_end',
                    name: 'last_time_end',
                    editable: false,
                    allowBlank: false,
                    submitValue: true,
                    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("last_time_start");
                            var end = Ext.getCmp("last_time_end");
                            var s_date = new Date(start.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, TIMETIP);
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        }
                    }
                },
                {
                    xtype: 'fieldcontainer',
                    layout: 'hbox',
                    defaults: {
                        labelWidth: 120,
                        width: 180
                    },
                    items: [
                         {
                             xtype: "numberfield",
                             fieldLabel: LASTCONSUMETIME,
                             id: 'last_time_interval',
                             allowBlank: false,
                             name: 'last_time_interval',
                             minValue: 0
                         },
                        {
                            xtype: 'displayfield',
                            value: LASTCONSUMETIMEMONTH
                        }
                    ]
                },
                {
                    xtype: 'displayfield',
                    value: LASTCONSUMETIMEMONTHINFO
                },
                {
                    xtype: 'combobox',
                    id: 'join_channel',
                    name: 'join_channel',
                    //store: UserSourceStore,
                    displayField: 'name',
                    valueField: 'id',
                    emptyText: SELECT,
                    queryMode: 'local',
                    fieldLabel: USERSOURCE

                }],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (Ext.getCmp("buy_times_max").getValue() < Ext.getCmp("buy_times_min").getValue()) {
                    Ext.Msg.alert(INFORMATION, BUYMNUMBERTIP);
                    return;
                }
                if (Ext.getCmp("buy_amount_max").getValue() < Ext.getCmp("buy_amount_min").getValue()) {
                    Ext.Msg.alert(INFORMATION, BUYAMOUNTTIP);
                    return;
                }
                if (form.isValid()) {
                    form.submit({
                        params: {
                            condition_id: condition_id,
                            condition_name: condition_name
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                condition_id = result.id;
                                UserConditionWin.close();
                                if (condition_id != null) {
                                    ShowConditionData(condition_id, TxtId);
                                } else {
                                    ShowConditionData(conditionID, TxtId);
                                }
                            }
                            else {
                                alert(ERRORSHOW + result.success);
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


    UserConditionWin = Ext.widget('window', {
        title: USERCONDINTION,
        closeAction: 'destroy',
        width: 400,
        layout: 'anchor',
        constrain: true,
        resizable: false,
        modal: true,
        items: form
    });

    UserConditionWin.show();
    loadData(condition_id);
}

function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                 // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                       // 获取日。
    return (new Date(s));                       // 返回日期。
}

ShowConditionData = function (conditionID, str) {

    condition_id = conditionID;
    if (conditionID != null && conditionID != "") {
        Ext.Ajax.request({
            url: '/PromotionsUser/UserConInfo',
            method: 'post',
            params: {
                condition_id: conditionID
            },
            success: function (form, action) {
                var resText = Ext.decode(form.responseText);
                if (resText.data[0].reg_start != "0") {
                    var reg_start = resText.data[0].reg_startDateTime;
                    //alert(resText.data[0].reg_startDateTime);
                }
                if (resText.data[0].reg_end != "0") {
                    var reg_end = resText.data[0].reg_endDateTime;
                }

                if (resText.data[0].reg_start != "0") {
                    var last_time_start = resText.data[0].last_time_startDateTime;
                }
                if (resText.data[0].last_time_end != "0") {
                    var last_time_end = resText.data[0].last_time_endDateTime;
                }

                ShowUserConInfo = JOINSTARTANDEND + reg_start + "--" + reg_end + "\n";
                ShowUserConInfo += JOINTIME + resText.data[0].reg_interval + "\n";
                ShowUserConInfo += BUYNUMBER + resText.data[0].buy_times_min + "--" + resText.data[0].buy_times_max + "\n";
                ShowUserConInfo += BUYAMOUNT + resText.data[0].buy_amount_min + "--" + resText.data[0].buy_amount_max + "\n";
                ShowUserConInfo += LASTSTARANDEND + last_time_start + "--" + last_time_end + "\n";
                ShowUserConInfo += LASTTIME + resText.data[0].last_time_interval;
                // ShowUserConInfo +="\n"+USERRESCOURE + resText.data[0].join_channel;

                Ext.getCmp(str).setValue(ShowUserConInfo);
            }
        });
    }
}
