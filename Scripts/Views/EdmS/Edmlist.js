Ext.define('gigade.EdmListConditionMain', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "elcm_name", type: "string" }
    ]
});

var ConditionNameStore = Ext.create('Ext.data.Store', {
    model: 'gigade.EdmListConditionMain',
    proxy: {
        type: 'ajax',
        url: '/EdmS/GetConditionList',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var genderStore = Ext.create('Ext.data.Store', {
    fields: ['gender', 'value'],
    data: [
    { "gender": "女性", "value": "0" },
    { "gender": "男性", "value": "1" }
    ]
});
var amountStore = Ext.create('Ext.data.Store', {
    fields: ['amount', 'avalue'],
    data: [
    { "amount": "大於", "avalue": "0" },
    { "amount": "小於", "avalue": "1" }
    ]
});
var zr = false;//是否點擊載入
Ext.onReady(function () {
    ConditionNameStore.load();
    var FrmLoad = Ext.create('Ext.form.Panel', {
        id: 'FrmLoad',
        layout: 'anchor',
        height: 70,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                id: 'conditionName',
                name: 'conditionName',
                labelWidth: 80,
                editable: true,
                store: ConditionNameStore,
                displayField: 'elcm_name',
                //valueField: 'elcm_id',
                fieldLabel: '選擇篩選條件',
                hidden: true,
                margin: '5 0 0 15',
                width: 480,
                queryMode: 'local',
                typeAhead: true,
                lastQuery: '',
                listeners: {
                    'beforequery': function () {
                        var name = Ext.getCmp("conditionName").getValue();
                        var length = 0;
                        if (name != null) {
                            length = name.length;
                        }
                        if (length == 0) {
                            ConditionNameStore.load();
                        }
                    }
                }
            },
            {
                xtype: 'button',
                text: "載入",
                id: 'btn_load',
                margin: '5 5 0 5',
                handler: loadListInfo
            },
            {
                xtype: 'button',
                text: '刪除',
                id: 'btn_delete',
                margin: '5 5 0 5',
                handler: deleteListInfo
            }
            ]
        }
        ]
    });
    var FrmCondition = Ext.create('Ext.form.Panel', {
        id: 'FrmCondition',
        layout: 'anchor',
        height: 250,
        bodyPadding: 15,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'panel',
            bodyStyle: "color:red;padding:5px;background:#EEEEEE",
            border: false,
            html: "注意事項：如果您挑選的條件過多，會導致數據加載過慢，請耐心等待。",
            width: 400,
            margin: '0 0 10 0'
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkGender',
                margin: '0 5 0 0'
            },
            {
                xtype: 'combobox',
                store: genderStore,
                id: 'ComboxGender',
                fieldLabel: '性別',// 0女1男
                displayField: 'gender',
                valueField: 'value',
                labelWidth: 80,
                width: 195,
                margin: '0 5 0 0',
                editable: false,
                value: 0
            },
            {
                xtype: 'checkbox',
                id: 'ChkBuy',
                margin: '0 5 0 100 '
            },
            {
                xtype: 'combobox',
                store: amountStore,
                id: 'ComboxBuy',
                fieldLabel: '購買次數',
                displayField: 'amount',
                valueField: 'avalue',
                labelWidth: 100,
                width: 185,
                margin: '0 5 0 0',
                editable: false,
                value: 0
            },
            {
                xtype: 'numberfield',
                id: 'NumBuyTimes',
                allowDecimals: false,
                width: 80,
                minvalue: 1,
                value: 1,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 2',
                value: '次',
                width: 30
            },
            {
                xtype: 'datefield',
                id: 'DfBuyTime1',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfBuyTime1").getValue();
                        var tend = Ext.getCmp("DfBuyTime2").getValue();
                        if (tend == null) {
                            Ext.getCmp("DfBuyTime2").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DfBuyTime1", "DfBuyTime2");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DfBuyTime2',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfBuyTime1").getValue();
                        var tend = Ext.getCmp("DfBuyTime2").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DfBuyTime1").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DfBuyTime1", "DfBuyTime2");
                    }
                }
            }

            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkAge',
                margin: '0 5 0 0'
            },
            {
                xtype: 'numberfield',
                id: 'NumAgeMin',
                fieldLabel: '年齡',
                labelWidth: 80,
                width: 130,
                // margin: '0 5 0 0',
                allowDecimals: false,
                minValue: 1,
                value: 1,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'numberfield',
                id: 'NumAgeMax',
                width: 45,
                allowDecimals: false,
                minValue: 1,
                value: 1,
                allowBlank: false

            },
            {
                xtype: 'checkbox',
                id: 'ChkCancel',
                margin: '0 5 0 106 '
            },
            {
                xtype: 'combobox',
                store: amountStore,
                id: 'ComboxCancel',
                fieldLabel: '取消次數',
                displayField: 'amount',
                valueField: 'avalue',
                labelWidth: 100,
                width: 185,
                margin: '0 5 0 0',
                editable: false,
                value: 0
            },
            {
                xtype: 'numberfield',
                id: 'NumCanceltimes',
                allowDecimals: false,
                width: 80,
                minvalue: 1,
                value: 1,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 2',
                value: '次',
                width: 30
            },
            {
                xtype: 'datefield',
                id: 'DfCancel1',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfCancel1").getValue();
                        var tend = Ext.getCmp("DfCancel2").getValue();
                        if (tend == null) {
                            Ext.getCmp("DfCancel2").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DfCancel1", "DfCancel2");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DfCancel2',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfCancel1").getValue();
                        var tend = Ext.getCmp("DfCancel2").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DfCancel1").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DfCancel1", "DfCancel2");
                    }
                }
            }

            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkRegisterTime',
                margin: '0 5 0 0'
            },
            {
                xtype: 'datefield',
                id: 'DFRegisterTimeMin',
                fieldLabel: '註冊時間',
                labelWidth: 80,
                width: 180,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFRegisterTimeMin").getValue();
                        var tend = Ext.getCmp("DFRegisterTimeMax").getValue();
                        if (tend == null) {
                            Ext.getCmp("DFRegisterTimeMax").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DFRegisterTimeMin", "DFRegisterTimeMax");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DFRegisterTimeMax',
                width: 95,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFRegisterTimeMin").getValue();
                        var tend = Ext.getCmp("DFRegisterTimeMax").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DFRegisterTimeMin").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DFRegisterTimeMin", "DFRegisterTimeMax");
                    }
                }
            },
            {
                xtype: 'checkbox',
                id: 'ChkReturn',
                margin: '0 5 0 56 '
            },
            {
                xtype: 'combobox',
                store: amountStore,
                id: 'ComboxReturn',
                fieldLabel: '退貨次數',
                displayField: 'amount',
                valueField: 'avalue',
                labelWidth: 100,
                width: 185,
                margin: '0 5 0 0',
                editable: false,
                value: 0
            },
            {
                xtype: 'numberfield',
                id: 'NumReturntimes',
                allowDecimals: false,
                width: 80,
                minvalue: 1,
                value: 1,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 2',
                value: '次',
                width: 30
            },
            {
                xtype: 'datefield',
                id: 'DfReturn1',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfReturn1").getValue();
                        var tend = Ext.getCmp("DfReturn2").getValue();
                        if (tend == null) {
                            Ext.getCmp("DfReturn2").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DfReturn1", "DfReturn2");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DfReturn2',
                width: 100,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DfReturn1").getValue();
                        var tend = Ext.getCmp("DfReturn2").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DfReturn1").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DfReturn1", "DfReturn2");
                    }
                }
            }

            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkLastOrder',
                margin: '0 5 0 0'
            },
            {
                xtype: 'datefield',
                id: 'DFLastOrderMin',
                fieldLabel: '最後訂單',
                labelWidth: 80,
                width: 180,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFLastOrderMin").getValue();
                        var tend = Ext.getCmp("DFLastOrderMax").getValue();
                        if (tend == null) {
                            Ext.getCmp("DFLastOrderMax").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DFLastOrderMin", "DFLastOrderMax");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DFLastOrderMax',
                width: 95,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFLastOrderMin").getValue();
                        var tend = Ext.getCmp("DFLastOrderMax").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DFLastOrderMin").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DFLastOrderMin", "DFLastOrderMax");
                    }
                }
            },
            {
                xtype: 'checkbox',
                id: 'ChkNotice',
                margin: '0 5 0 56 '
            },
            {
                xtype: 'combobox',
                store: amountStore,
                id: 'ComboxNotice',
                fieldLabel: '貨到通知次數',
                displayField: 'amount',
                valueField: 'avalue',
                labelWidth: 100,
                width: 185,
                margin: '0 5 0 0',
                editable: false,
                value: 0
            },
            {
                xtype: 'numberfield',
                id: 'NumNotice',
                allowDecimals: false,
                width: 80,
                minvalue: 1,
                value: 1,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 2',
                value: '次',
                width: 30
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkLastLogin',
                margin: '0 5 0 0'
            },
            {
                xtype: 'datefield',
                id: 'DFLastLoginMin',
                fieldLabel: '最後登入',
                labelWidth: 80,
                width: 180,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFLastLoginMin").getValue();
                        var tend = Ext.getCmp("DFLastLoginMax").getValue();
                        if (tend == null) {
                            Ext.getCmp("DFLastLoginMax").setValue(setNextMonth(tstart, 1));
                        }
                        compareSToE("DFLastLoginMin", "DFLastLoginMax");
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'datefield',
                id: 'DFLastLoginMax',
                width: 95,
                minValue: '2010-01-01',
                listeners: {
                    select: function () {
                        var tstart = Ext.getCmp("DFLastLoginMin").getValue();
                        var tend = Ext.getCmp("DFLastLoginMax").getValue();
                        if (tstart == null) {
                            Ext.getCmp("DFLastLoginMin").setValue(setNextMonth(tend, -1));
                        }
                        compareEToS("DFLastLoginMin", "DFLastLoginMax");
                    }
                }
            },
            {
                xtype: 'checkbox',
                id: 'ChkTotalConsumption',
                margin: '0 5 0 56 '
            },
            {
                xtype: 'numberfield',
                id: 'NumTotalConsumption1',
                fieldLabel: '消費累計金額',
                allowDecimals: false,
                minvalue: 1,
                labelWidth: 100,
                width: 210,
                allowBlank: false
            },

            {
                xtype: 'displayfield',
                margin: '0 2 0 2',
                value: '～',
                width: 10
            },
            {
                xtype: 'numberfield',
                id: 'NumTotalConsumption2',
                allowDecimals: false,
                width: 100,
                minvalue: 1,
                allowBlank: false
            },

            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'checkbox',
                id: 'ChkBlackList',
                margin: '0 0 0 214',
                checked: true,
                handler: function () {
                    if (this.checked == false && zr != true) {
                        Ext.MessageBox.confirm(CONFIRM, "確定取消排除黑名單？", function (btn) {
                            if (btn == "yes") {
                                Ext.getCmp("ChkBlackList").setValue(false);
                            } else {
                                Ext.getCmp("ChkBlackList").setValue(true);
                            }
                        });
                    }                 
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 0 0 5',
                value: '排除黑名單用戶',
                width: 100
            },
            {
                xtype: 'button',
                margin: '0 5 0 155',
                text: "儲存條件",
                id: 'BtnSave',
                handler: saveConditions
            },
            {
                xtype: 'button',
                text: '重置',
                id: 'BtnReset',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    this.up('form').getForm().reset();
                    Ext.getCmp("conditionName").reset();
                    Ext.getCmp("show").reset();
                }
            }
            ]
        }
        ]
    });
    var FrmSummary = Ext.create('Ext.form.Panel', {
        id: 'FrmSummary',
        layout: 'anchor',
        height: 60,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'displayfield',
                id: 'summary',
                labelWidth: 80,
                fieldLabel: '名單總計',
                margin: '5 0 0 5'
            },
            {
                xtype: 'displayfield',
                id: 'show',
                labelWidth: 100,
                value: '---------',
                margin: '5 0 0 5'
            },
            {
                xtype: 'displayfield',
                value: '名',
                margin: '5 0 0 20'
            },
            {
                xtype: 'button',
                text: " 查詢 ",
                id: 'btnQuery',
                margin: '5 5 0 340',
                handler: Query
            },
            {
                xtype: 'button',
                text: '下載CSV',
                id: 'btnDownLoad',
                margin: '5 5 0 5',
                handler: Export
            }
            ]
        }
        ]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [FrmLoad, FrmCondition, FrmSummary],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
compareSToE = function (a, b) {
    var little = Ext.getCmp("" + a + "").getValue();
    var big = Ext.getCmp("" + b + "").getValue();
    if (little >= big) {
        Ext.Msg.alert(INFORMATION, "開始日期不能大於結束日期");
        Ext.getCmp("" + b + "").setValue(setNextMonth(little, 1));
    }
}
compareEToS = function (a, b) {
    var little = Ext.getCmp("" + a + "").getValue();
    var big = Ext.getCmp("" + b + "").getValue();
    if (big <= little) {
        Ext.Msg.alert(INFORMATION, "結束日期不能小於開始日期");
        Ext.getCmp("" + a + "").setValue(setNextMonth(big, -1));
    }
}
compareBeforeToAfter = function (a, b) {
    var little = Ext.getCmp("" + a + "").getValue();
    var big = Ext.getCmp("" + b + "").getValue();
    if (big <= little) {
        Ext.getCmp("" + a + "").setValue(big);
        Ext.getCmp("" + b + "").setValue(little);
    }
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    return s;
}
loadListInfo = function () {
    zr = true;
    Ext.getCmp("FrmCondition").getForm().reset();
    var conditionName = Ext.getCmp("conditionName").getValue();
    if (conditionName != "") {
        Ext.Ajax.request({
            url: '/EdmS/LoadCondition',
            method: 'post',
            params: {
                conditionName: conditionName
            },
            success: function (form, action) {
                var store = Ext.decode(form.responseText).data;
                for (var i = 0; i < store.length; i++) {
                    if (store[i].chkGender == true) {
                        Ext.getCmp("ChkGender").setValue(true);
                        Ext.getCmp("ComboxGender").setValue(store[i].genderCondition);
                    }
                    if (store[i].ChkBuy == true) {
                        Ext.getCmp("ChkBuy").setValue(true);
                        Ext.getCmp("ComboxBuy").setValue(store[i].buyCondition);
                        Ext.getCmp("NumBuyTimes").setValue(store[i].buyTimes);
                        if (store[i].buyTimeMin != "0001-01-01") {
                            Ext.getCmp("DfBuyTime1").setValue(store[i].buyTimeMin);
                        }
                        if (store[i].buyTimeMax != "0001-01-01") {
                            Ext.getCmp("DfBuyTime2").setValue(store[i].buyTimeMax);
                        }
                    }
                    if (store[i].ChkAge == true) {
                        Ext.getCmp("ChkAge").setValue(true);
                        Ext.getCmp("NumAgeMin").setValue(store[i].ageMin);
                        Ext.getCmp("NumAgeMax").setValue(store[i].ageMax);
                    }
                    if (store[i].ChkCancel == true) {
                        Ext.getCmp("ChkCancel").setValue(true);
                        Ext.getCmp("ComboxCancel").setValue(store[i].cancelCondition);
                        Ext.getCmp("NumCanceltimes").setValue(store[i].cancelTimes);
                        if (store[i].cancelTimeMin != "0001-01-01") {
                            Ext.getCmp("DfCancel1").setValue(store[i].cancelTimeMin);
                        }
                        if (store[i].cancelTimeMax != "0001-01-01") {
                            Ext.getCmp("DfCancel2").setValue(store[i].cancelTimeMax);
                        }
                    }
                    if (store[i].ChkRegisterTime == true) {
                        Ext.getCmp("ChkRegisterTime").setValue(true);
                        if (store[i].registerTimeMin != "0001-01-01") {
                            Ext.getCmp("DFRegisterTimeMin").setValue(store[i].registerTimeMin);
                        }
                        if (store[i].registerTimeMax != "0001-01-01") {
                            Ext.getCmp("DFRegisterTimeMax").setValue(store[i].registerTimeMax);
                        }
                    }
                    if (store[i].ChkReturn == true) {
                        Ext.getCmp("ChkReturn").setValue(true);
                        Ext.getCmp("ComboxReturn").setValue(store[i].returnCondition);
                        Ext.getCmp("NumReturntimes").setValue(store[i].returnTimes);
                        if (store[i].returnTimeMin != "0001-01-01") {
                            Ext.getCmp("DfReturn1").setValue(store[i].returnTimeMin);
                        }
                        if (store[i].returnTimeMax != "0001-01-01") {
                            Ext.getCmp("DfReturn2").setValue(store[i].returnTimeMax);
                        }
                    }
                    if (store[i].ChkLastOrder == true) {
                        Ext.getCmp("ChkLastOrder").setValue(true);
                        if (store[i].lastOrderMin != "0001-01-01") {
                            Ext.getCmp("DFLastOrderMin").setValue(store[i].lastOrderMin);
                        }
                        if (store[i].lastOrderMax != "0001-01-01") {
                            Ext.getCmp("DFLastOrderMax").setValue(store[i].lastOrderMax);
                        }
                    }
                    if (store[i].ChkNotice == true) {
                        Ext.getCmp("ChkNotice").setValue(true);
                        Ext.getCmp("ComboxNotice").setValue(store[i].noticeCondition);
                        Ext.getCmp("NumNotice").setValue(store[i].noticeTimes);
                    }
                    if (store[i].ChkLastLogin == true) {
                        Ext.getCmp("ChkLastLogin").setValue(true);
                        if (store[i].lastLoginMin != "0001-01-01") {
                            Ext.getCmp("DFLastLoginMin").setValue(store[i].lastLoginMin);
                        }
                        if (store[i].lastLoginMax != "0001-01-01") {
                            Ext.getCmp("DFLastLoginMax").setValue(store[i].lastLoginMax);
                        }
                    }
                    if (store[i].ChkTotalConsumption == true) {
                        Ext.getCmp("ChkTotalConsumption").setValue(true);
                        if (store[i].totalConsumptionMin != 0) {
                        Ext.getCmp("NumTotalConsumption1").setValue(store[i].totalConsumptionMin);
                        }
                        if (store[i].totalConsumptionMax != 0) {
                        Ext.getCmp("NumTotalConsumption2").setValue(store[i].totalConsumptionMax);
                    }
                    }
                    if (store[i].ChkBlackList != true) {
                        Ext.getCmp("ChkBlackList").setValue(false);
                    }
                    zr = false;                 
                }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, "載入失敗");
            }
        });    
    }
    else {
        Ext.Msg.alert(INFORMATION, "請選擇篩選條件");
    }
}
deleteListInfo = function () {
    var conditionName = Ext.getCmp("conditionName").getValue();
    if (conditionName == "" || conditionName == undefined || conditionName == null) {
        Ext.Msg.alert(INFORMATION, "請輸入篩選條件");
    }
    else {
        ConditionNameStore.removeAll();
        Ext.Ajax.request({
            url: '/EdmS/DeleteListInfo',
            method: 'post',
            params: {
                elcm_name: conditionName
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.getCmp("conditionName").reset();
                    Ext.Msg.alert(INFORMATION, "刪除成功!");
                    Ext.getCmp("FrmCondition").getForm().reset();

                }
                else {
                    Ext.Msg.alert(INFORMATION, "操作失敗!");
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        })
    }
}
saveConditions = function () {
    var chkGender = Ext.getCmp("ChkGender").getValue();
    if (chkGender) {
        var genderCondition = Ext.getCmp("ComboxGender").getValue();
    }
    var ChkBuy = Ext.getCmp("ChkBuy").getValue();
    if (ChkBuy) {
        var buyCondition = Ext.getCmp("ComboxBuy").getValue();
        var buyTimes = Ext.getCmp("NumBuyTimes").getValue();
        var buyTimeMin = Ext.getCmp("DfBuyTime1").getValue();
        var buyTimeMax = Ext.getCmp("DfBuyTime2").getValue();
    }
    var ChkAge = Ext.getCmp("ChkAge").getValue();
    if (ChkAge) {
        compareBeforeToAfter("NumAgeMin", "NumAgeMax");
        var ageMin = Ext.getCmp("NumAgeMin").getValue();
        var ageMax = Ext.getCmp("NumAgeMax").getValue();
    }
    var ChkCancel = Ext.getCmp("ChkCancel").getValue();
    if (ChkCancel) {
        var cancelCondition = Ext.getCmp("ComboxCancel").getValue();
        var cancelTimes = Ext.getCmp("NumCanceltimes").getValue();
        var cancelTimeMin = Ext.getCmp("DfCancel1").getValue();
        var cancelTimeMax = Ext.getCmp("DfCancel2").getValue();
    }
    var ChkRegisterTime = Ext.getCmp("ChkRegisterTime").getValue();
    if (ChkRegisterTime) {
        var registerTimeMin = Ext.getCmp("DFRegisterTimeMin").getValue();
        var registerTimeMax = Ext.getCmp("DFRegisterTimeMax").getValue();
    }
    var ChkReturn = Ext.getCmp("ChkReturn").getValue();
    if (ChkReturn) {
        var returnCondition = Ext.getCmp("ComboxReturn").getValue();
        var returnTimes = Ext.getCmp("NumReturntimes").getValue();
        var returnTimeMin = Ext.getCmp("DfReturn1").getValue();
        var returnTimeMax = Ext.getCmp("DfReturn2").getValue();
    }
    var ChkLastOrder = Ext.getCmp("ChkLastOrder").getValue();
    if (ChkLastOrder) {
        var lastOrderMin = Ext.getCmp("DFLastOrderMin").getValue();
        var lastOrderMax = Ext.getCmp("DFLastOrderMax").getValue();
    }
    var ChkNotice = Ext.getCmp("ChkNotice").getValue();
    if (ChkNotice) {
        var noticeCondition = Ext.getCmp("ComboxNotice").getValue();
        var noticeTimes = Ext.getCmp("NumNotice").getValue();
    }
    var ChkLastLogin = Ext.getCmp("ChkLastLogin").getValue();
    if (ChkLastLogin) {
        var lastLoginMin = Ext.getCmp("DFLastLoginMin").getValue();
        var lastLoginMax = Ext.getCmp("DFLastLoginMax").getValue();
    }
    var ChkTotalConsumption = Ext.getCmp("ChkTotalConsumption").getValue();
    if (ChkTotalConsumption) {
        compareBeforeToAfter("NumTotalConsumption1", "NumTotalConsumption2");
        var totalConsumptionMin = Ext.getCmp("NumTotalConsumption1").getValue();
        var totalConsumptionMax = Ext.getCmp("NumTotalConsumption2").getValue();
    }
    var ChkBlackList = Ext.getCmp("ChkBlackList").getValue();
    var conditionName = Ext.getCmp("conditionName").getValue();
    if (conditionName == "" || conditionName == undefined || conditionName == null) {       
        SaveNameFunction(ConditionNameStore);
    }
    else {
        if (chkGender || ChkBuy || ChkAge || ChkCancel || ChkRegisterTime || ChkReturn || ChkLastOrder || ChkNotice || ChkLastLogin || ChkTotalConsumption || ChkBlackList) {
            Ext.Ajax.request({
                url: '/EdmS/SaveListInfo',
                method: 'post',                
                params: {
                    elcm_name: conditionName,
                    chkGender: chkGender,
                    genderCondition: genderCondition,
                    ChkBuy: ChkBuy,
                    buyCondition: buyCondition,
                    buyTimes: buyTimes,
                    buyTimeMin: buyTimeMin,
                    buyTimeMax: buyTimeMax,
                    ChkAge: ChkAge,
                    ageMin: ageMin,
                    ageMax: ageMax,
                    ChkCancel: ChkCancel,
                    cancelCondition: cancelCondition,
                    cancelTimes: cancelTimes,
                    cancelTimeMin: cancelTimeMin,
                    cancelTimeMax: cancelTimeMax,
                    ChkRegisterTime: ChkRegisterTime,
                    registerTimeMin: registerTimeMin,
                    registerTimeMax: registerTimeMax,
                    ChkReturn: ChkReturn,
                    returnCondition: returnCondition,
                    returnTimes: returnTimes,
                    returnTimeMin: returnTimeMin,
                    returnTimeMax: returnTimeMax,
                    ChkLastOrder: ChkLastOrder,
                    lastOrderMin: lastOrderMin,
                    lastOrderMax: lastOrderMax,
                    ChkNotice: ChkNotice,
                    noticeCondition: noticeCondition,
                    noticeTimes: noticeTimes,
                    ChkLastLogin: ChkLastLogin,
                    lastLoginMin: lastLoginMin,
                    lastLoginMax: lastLoginMax,
                    ChkTotalConsumption: ChkTotalConsumption,
                    totalConsumptionMin: totalConsumptionMin,
                    totalConsumptionMax: totalConsumptionMax,
                    ChkBlackList: ChkBlackList
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        ConditionNameStore.removeAll();
                        Ext.getCmp("conditionName").reset();
                        Ext.Msg.alert(INFORMATION, "保存成功!");
                        ConditionNameStore.load();
                        Ext.getCmp("show").reset();
                    }
                    else {
                        if (result.msg == '0') {
                            Ext.Msg.alert(INFORMATION, "保存篩選條件名稱失敗");
                        }
                        else if (result.msg == '1') {
                            Ext.MessageBox.confirm(CONFIRM, "篩選條件名稱已存在,是否覆蓋原條件？", function (btn) {
                                if (btn == "yes") {
                                    ok();
                                    Ext.getCmp("conditionName").reset();
                                    Ext.getCmp("show").reset();
                                }
                            });
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    }
                },
                failure: function (form, action) {
                    if (Ext.decode(action.response.responseText).msg == '0') {
                        Ext.Msg.alert(INFORMATION, "保存篩選條件名稱失敗");
                    }
                    else if (Ext.decode(action.response.responseText).msg == '1') {
                        Ext.Msg.alert(INFORMATION, "篩選條件名稱已存在！");
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                }
            })
        }
        else {
            Ext.Msg.alert(INFORMATION, "請勾選條件");
        }
    }
}
Query = function () {
    Ext.getCmp("btnQuery").setDisabled(true);
    var chkGender = Ext.getCmp("ChkGender").getValue();
    if (chkGender) {
        var genderCondition = Ext.getCmp("ComboxGender").getValue();
    }
    var ChkBuy = Ext.getCmp("ChkBuy").getValue();
    if (ChkBuy) {
        var buyCondition = Ext.getCmp("ComboxBuy").getValue();
        var buyTimes = Ext.getCmp("NumBuyTimes").getValue();
        var buyTimeMin = Ext.getCmp("DfBuyTime1").getValue();
        var buyTimeMax = Ext.getCmp("DfBuyTime2").getValue();
    }
    var ChkAge = Ext.getCmp("ChkAge").getValue();
    if (ChkAge) {
        compareBeforeToAfter("NumAgeMin", "NumAgeMax");
        var ageMin = Ext.getCmp("NumAgeMin").getValue();
        var ageMax = Ext.getCmp("NumAgeMax").getValue();
    }
    var ChkCancel = Ext.getCmp("ChkCancel").getValue();
    if (ChkCancel) {
        var cancelCondition = Ext.getCmp("ComboxCancel").getValue();
        var cancelTimes = Ext.getCmp("NumCanceltimes").getValue();
        var cancelTimeMin = Ext.getCmp("DfCancel1").getValue();
        var cancelTimeMax = Ext.getCmp("DfCancel2").getValue();
    }
    var ChkRegisterTime = Ext.getCmp("ChkRegisterTime").getValue();
    if (ChkRegisterTime) {
        var registerTimeMin = Ext.getCmp("DFRegisterTimeMin").getValue();
        var registerTimeMax = Ext.getCmp("DFRegisterTimeMax").getValue();
    }
    var ChkReturn = Ext.getCmp("ChkReturn").getValue();
    if (ChkReturn) {
        var returnCondition = Ext.getCmp("ComboxReturn").getValue();
        var returnTimes = Ext.getCmp("NumReturntimes").getValue();
        var returnTimeMin = Ext.getCmp("DfReturn1").getValue();
        var returnTimeMax = Ext.getCmp("DfReturn2").getValue();
    }
    var ChkLastOrder = Ext.getCmp("ChkLastOrder").getValue();
    if (ChkLastOrder) {
        var lastOrderMin = Ext.getCmp("DFLastOrderMin").getValue();
        var lastOrderMax = Ext.getCmp("DFLastOrderMax").getValue();
    }
    var ChkNotice = Ext.getCmp("ChkNotice").getValue();
    if (ChkNotice) {
        var noticeCondition = Ext.getCmp("ComboxNotice").getValue();
        var noticeTimes = Ext.getCmp("NumNotice").getValue();
    }
    var ChkLastLogin = Ext.getCmp("ChkLastLogin").getValue();
    if (ChkLastLogin) {
        var lastLoginMin = Ext.getCmp("DFLastLoginMin").getValue();
        var lastLoginMax = Ext.getCmp("DFLastLoginMax").getValue();
    }
    var ChkTotalConsumption = Ext.getCmp("ChkTotalConsumption").getValue();
    if (ChkTotalConsumption) {
        compareBeforeToAfter("NumTotalConsumption1", "NumTotalConsumption2");
        var totalConsumptionMin = Ext.getCmp("NumTotalConsumption1").getValue();
        var totalConsumptionMax = Ext.getCmp("NumTotalConsumption2").getValue();
    }
    var ChkBlackList = Ext.getCmp("ChkBlackList").getValue();
    if (chkGender || ChkBuy || ChkAge || ChkCancel || ChkRegisterTime || ChkReturn || ChkLastOrder || ChkNotice || ChkLastLogin || ChkTotalConsumption || ChkBlackList) {
        Ext.Ajax.request({
            url: '/EdmS/GetUserNum',
            method: 'post',
            params: {
                chkGender: chkGender,
                genderCondition: genderCondition,
                ChkBuy: ChkBuy,
                buyCondition: buyCondition,
                buyTimes: buyTimes,
                buyTimeMin: buyTimeMin,
                buyTimeMax: buyTimeMax,
                ChkAge: ChkAge,
                ageMin: ageMin,
                ageMax: ageMax,
                ChkCancel: ChkCancel,
                cancelCondition: cancelCondition,
                cancelTimes: cancelTimes,
                cancelTimeMin: cancelTimeMin,
                cancelTimeMax: cancelTimeMax,
                ChkRegisterTime: ChkRegisterTime,
                registerTimeMin: registerTimeMin,
                registerTimeMax: registerTimeMax,
                ChkReturn: ChkReturn,
                returnCondition: returnCondition,
                returnTimes: returnTimes,
                returnTimeMin: returnTimeMin,
                returnTimeMax: returnTimeMax,
                ChkLastOrder: ChkLastOrder,
                lastOrderMin: lastOrderMin,
                lastOrderMax: lastOrderMax,
                ChkNotice: ChkNotice,
                noticeCondition: noticeCondition,
                noticeTimes: noticeTimes,
                ChkLastLogin: ChkLastLogin,
                lastLoginMin: lastLoginMin,
                lastLoginMax: lastLoginMax,
                ChkTotalConsumption: ChkTotalConsumption,
                totalConsumptionMin: totalConsumptionMin,
                totalConsumptionMax: totalConsumptionMax,
                ChkBlackList: ChkBlackList
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.getCmp("show").setValue(result.totalCount);
                    Ext.getCmp("btnQuery").setDisabled(false);
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        })
    }
    else {
        Ext.Msg.alert(INFORMATION, "請勾選條件");
    }
}
Export = function () {
    var chkGender = Ext.getCmp("ChkGender").getValue();
    if (chkGender) {
        var genderCondition = Ext.getCmp("ComboxGender").getValue();
    }
    var ChkBuy = Ext.getCmp("ChkBuy").getValue();
    if (ChkBuy) {
        var buyCondition = Ext.getCmp("ComboxBuy").getValue();
        var buyTimes = Ext.getCmp("NumBuyTimes").getValue();
        var buyTimeMin = Ext.getCmp("DfBuyTime1").getValue();
        var buyTimeMax = Ext.getCmp("DfBuyTime2").getValue();
    }
    var ChkAge = Ext.getCmp("ChkAge").getValue();
    if (ChkAge) {
        compareBeforeToAfter("NumAgeMin", "NumAgeMax");
        var ageMin = Ext.getCmp("NumAgeMin").getValue();
        var ageMax = Ext.getCmp("NumAgeMax").getValue();
    }
    var ChkCancel = Ext.getCmp("ChkCancel").getValue();
    if (ChkCancel) {
        var cancelCondition = Ext.getCmp("ComboxCancel").getValue();
        var cancelTimes = Ext.getCmp("NumCanceltimes").getValue();
        var cancelTimeMin = Ext.getCmp("DfCancel1").getValue();
        var cancelTimeMax = Ext.getCmp("DfCancel2").getValue();
    }
    var ChkRegisterTime = Ext.getCmp("ChkRegisterTime").getValue();
    if (ChkRegisterTime) {
        var registerTimeMin = Ext.getCmp("DFRegisterTimeMin").getValue();
        var registerTimeMax = Ext.getCmp("DFRegisterTimeMax").getValue();
    }
    var ChkReturn = Ext.getCmp("ChkReturn").getValue();
    if (ChkReturn) {
        var returnCondition = Ext.getCmp("ComboxReturn").getValue();
        var returnTimes = Ext.getCmp("NumReturntimes").getValue();
        var returnTimeMin = Ext.getCmp("DfReturn1").getValue();
        var returnTimeMax = Ext.getCmp("DfReturn2").getValue();
    }
    var ChkLastOrder = Ext.getCmp("ChkLastOrder").getValue();
    if (ChkLastOrder) {
        var lastOrderMin = Ext.getCmp("DFLastOrderMin").getValue();
        var lastOrderMax = Ext.getCmp("DFLastOrderMax").getValue();
    }
    var ChkNotice = Ext.getCmp("ChkNotice").getValue();
    if (ChkNotice) {
        var noticeCondition = Ext.getCmp("ComboxNotice").getValue();
        var noticeTimes = Ext.getCmp("NumNotice").getValue();
    }
    var ChkLastLogin = Ext.getCmp("ChkLastLogin").getValue();
    if (ChkLastLogin) {
        var lastLoginMin = Ext.getCmp("DFLastLoginMin").getValue();
        var lastLoginMax = Ext.getCmp("DFLastLoginMax").getValue();
    }
    var ChkTotalConsumption = Ext.getCmp("ChkTotalConsumption").getValue();
    if (ChkTotalConsumption) {
        compareBeforeToAfter("NumTotalConsumption1", "NumTotalConsumption2");
        var totalConsumptionMin = Ext.getCmp("NumTotalConsumption1").getValue();
        var totalConsumptionMax = Ext.getCmp("NumTotalConsumption2").getValue();
    }
    var ChkBlackList = Ext.getCmp("ChkBlackList").getValue();
    if (chkGender || ChkBuy || ChkAge || ChkCancel || ChkRegisterTime || ChkReturn || ChkLastOrder || ChkNotice || ChkLastLogin || ChkTotalConsumption || ChkBlackList) {
        Ext.MessageBox.show({
            msg: 'Loading....',
            wait: true
        });
        Ext.Ajax.request({
            url: '/EdmS/Export',
            method: 'post',
            timeout: 900000,
            params: {
                chkGender: chkGender,
                genderCondition: genderCondition,
                ChkBuy: ChkBuy,
                buyCondition: buyCondition,
                buyTimes: buyTimes,
                buyTimeMin: buyTimeMin,
                buyTimeMax: buyTimeMax,
                ChkAge: ChkAge,
                ageMin: ageMin,
                ageMax: ageMax,
                ChkCancel: ChkCancel,
                cancelCondition: cancelCondition,
                cancelTimes: cancelTimes,
                cancelTimeMin: cancelTimeMin,
                cancelTimeMax: cancelTimeMax,
                ChkRegisterTime: ChkRegisterTime,
                registerTimeMin: registerTimeMin,
                registerTimeMax: registerTimeMax,
                ChkReturn: ChkReturn,
                returnCondition: returnCondition,
                returnTimes: returnTimes,
                returnTimeMin: returnTimeMin,
                returnTimeMax: returnTimeMax,
                ChkLastOrder: ChkLastOrder,
                lastOrderMin: lastOrderMin,
                lastOrderMax: lastOrderMax,
                ChkNotice: ChkNotice,
                noticeCondition: noticeCondition,
                noticeTimes: noticeTimes,
                ChkLastLogin: ChkLastLogin,
                lastLoginMin: lastLoginMin,
                lastLoginMax: lastLoginMax,
                ChkTotalConsumption: ChkTotalConsumption,
                totalConsumptionMin: totalConsumptionMin,
                totalConsumptionMax: totalConsumptionMax,
                ChkBlackList: ChkBlackList
            },
            success: function (form, action) {
                Ext.MessageBox.hide();
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    window.location = '../../ImportUserIOExcel/' + result.fileName;
                } else {
                    Ext.MessageBox.hide();
                    Ext.Msg.alert("提示信息", "匯出失敗或沒有數據！");
                }
            },
            failure: function (form, action) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        })
    }
    else {
        Ext.Msg.alert(INFORMATION, "請勾選條件");
    }
}
function ok() {
    Ext.Ajax.request({
        url: '/EdmS/UpdateCondition',
        method: 'post',
        params: {
            elcm_name: Ext.getCmp("conditionName").getValue(),
            chkGender: Ext.getCmp("ChkGender").getValue(),
            genderCondition: Ext.getCmp("ComboxGender").getValue(),
            ChkBuy: Ext.getCmp("ChkBuy").getValue(),
            buyCondition: Ext.getCmp("ComboxBuy").getValue(),
            buyTimes: Ext.getCmp("NumBuyTimes").getValue(),
            buyTimeMin: Ext.getCmp("DfBuyTime1").getValue(),
            buyTimeMax: Ext.getCmp("DfBuyTime2").getValue(),
            ChkAge: Ext.getCmp("ChkAge").getValue(),
            ageMin: Ext.getCmp("NumAgeMin").getValue(),
            ageMax: Ext.getCmp("NumAgeMax").getValue(),
            ChkCancel: Ext.getCmp("ChkCancel").getValue(),
            cancelCondition: Ext.getCmp("ComboxCancel").getValue(),
            cancelTimes: Ext.getCmp("NumCanceltimes").getValue(),
            cancelTimeMin: Ext.getCmp("DfCancel1").getValue(),
            cancelTimeMax: Ext.getCmp("DfCancel2").getValue(),
            ChkRegisterTime: Ext.getCmp("ChkRegisterTime").getValue(),
            registerTimeMin: Ext.getCmp("DFRegisterTimeMin").getValue(),
            registerTimeMax: Ext.getCmp("DFRegisterTimeMax").getValue(),
            ChkReturn: Ext.getCmp("ChkReturn").getValue(),
            returnCondition: Ext.getCmp("ComboxReturn").getValue(),
            returnTimes: Ext.getCmp("NumReturntimes").getValue(),
            returnTimeMin: Ext.getCmp("DfReturn1").getValue(),
            returnTimeMax: Ext.getCmp("DfReturn2").getValue(),
            ChkLastOrder: Ext.getCmp("ChkLastOrder").getValue(),
            lastOrderMin: Ext.getCmp("DFLastOrderMin").getValue(),
            lastOrderMax: Ext.getCmp("DFLastOrderMax").getValue(),
            ChkNotice: Ext.getCmp("ChkNotice").getValue(),
            noticeCondition: Ext.getCmp("ComboxNotice").getValue(),
            noticeTimes: Ext.getCmp("NumNotice").getValue(),
            ChkLastLogin: Ext.getCmp("ChkLastLogin").getValue(),
            lastLoginMin: Ext.getCmp("DFLastLoginMin").getValue(),
            lastLoginMax: Ext.getCmp("DFLastLoginMax").getValue(),
            ChkTotalConsumption: Ext.getCmp("ChkTotalConsumption").getValue(),
            totalConsumptionMin: Ext.getCmp("NumTotalConsumption1").getValue(),
            totalConsumptionMax: Ext.getCmp("NumTotalConsumption2").getValue(),
            ChkBlackList: Ext.getCmp("ChkBlackList").getValue()
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, "保存成功!");
                ConditionNameStore.removeAll();
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

SaveNameFunction = function (store) {
    var saveFrm = Ext.create('Ext.form.Panel', {
        id: 'saveFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EdmS/SaveListInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '篩選條件名稱',
            id: 'edmconditionName',
            name: 'edmconditionName',
            margin: '0 0 0 15'
        }
        ],
        buttons: [
        {
            formBind: true,
            disabled: true,
            text: '保存',
            id:'btn_saveName',
            margin: '0 10 0 0',
            handler: function () {
                var numRegex = /^[-+]?[\d]+$/;
                var edmname = Ext.getCmp('edmconditionName').getValue();
                if (edmname == "" || edmname == undefined || edmname == null) {
                    Ext.Msg.alert(INFORMATION, "請輸入篩選條件名稱");
                }
                else if (numRegex.test(edmname)) {
                    Ext.Msg.alert(INFORMATION, "篩選條件名稱不可只輸入數字");
                }
                else {
                    var chkGender = Ext.getCmp("ChkGender").getValue();
                    if (chkGender) {
                        var genderCondition = Ext.getCmp("ComboxGender").getValue();
                    }
                    var ChkBuy = Ext.getCmp("ChkBuy").getValue();
                    if (ChkBuy) {
                        var buyCondition = Ext.getCmp("ComboxBuy").getValue();
                        var buyTimes = Ext.getCmp("NumBuyTimes").getValue();
                        var buyTimeMin = Ext.getCmp("DfBuyTime1").getValue();
                        var buyTimeMax = Ext.getCmp("DfBuyTime2").getValue();
                    }
                    var ChkAge = Ext.getCmp("ChkAge").getValue();
                    if (ChkAge) {
                        compareBeforeToAfter("NumAgeMin", "NumAgeMax");
                        var ageMin = Ext.getCmp("NumAgeMin").getValue();
                        var ageMax = Ext.getCmp("NumAgeMax").getValue();
                    }
                    var ChkCancel = Ext.getCmp("ChkCancel").getValue();
                    if (ChkCancel) {
                        var cancelCondition = Ext.getCmp("ComboxCancel").getValue();
                        var cancelTimes = Ext.getCmp("NumCanceltimes").getValue();
                        var cancelTimeMin = Ext.getCmp("DfCancel1").getValue();
                        var cancelTimeMax = Ext.getCmp("DfCancel2").getValue();
                    }
                    var ChkRegisterTime = Ext.getCmp("ChkRegisterTime").getValue();
                    if (ChkRegisterTime) {
                        var registerTimeMin = Ext.getCmp("DFRegisterTimeMin").getValue();
                        var registerTimeMax = Ext.getCmp("DFRegisterTimeMax").getValue();
                    }
                    var ChkReturn = Ext.getCmp("ChkReturn").getValue();
                    if (ChkReturn) {
                        var returnCondition = Ext.getCmp("ComboxReturn").getValue();
                        var returnTimes = Ext.getCmp("NumReturntimes").getValue();
                        var returnTimeMin = Ext.getCmp("DfReturn1").getValue();
                        var returnTimeMax = Ext.getCmp("DfReturn2").getValue();
                    }
                    var ChkLastOrder = Ext.getCmp("ChkLastOrder").getValue();
                    if (ChkLastOrder) {
                        var lastOrderMin = Ext.getCmp("DFLastOrderMin").getValue();
                        var lastOrderMax = Ext.getCmp("DFLastOrderMax").getValue();
                    }
                    var ChkNotice = Ext.getCmp("ChkNotice").getValue();
                    if (ChkNotice) {
                        var noticeCondition = Ext.getCmp("ComboxNotice").getValue();
                        var noticeTimes = Ext.getCmp("NumNotice").getValue();
                    }
                    var ChkLastLogin = Ext.getCmp("ChkLastLogin").getValue();
                    if (ChkLastLogin) {
                        var lastLoginMin = Ext.getCmp("DFLastLoginMin").getValue();
                        var lastLoginMax = Ext.getCmp("DFLastLoginMax").getValue();
                    }
                    var ChkTotalConsumption = Ext.getCmp("ChkTotalConsumption").getValue();
                    if (ChkTotalConsumption) {
                        compareBeforeToAfter("NumTotalConsumption1", "NumTotalConsumption2");
                        var totalConsumptionMin = Ext.getCmp("NumTotalConsumption1").getValue();
                        var totalConsumptionMax = Ext.getCmp("NumTotalConsumption2").getValue();
                    }
                    var ChkBlackList = Ext.getCmp("ChkBlackList").getValue();
                    if (ChkGender || ChkBuy || ChkAge || ChkCancel || ChkRegisterTime || ChkReturn || ChkLastOrder || ChkNotice || ChkLastLogin || ChkTotalConsumption || ChkBlackList) {
                        var form = this.up('form').getForm();
                        this.disable();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                params: {
                                    elcm_name: edmname,
                                    chkGender: chkGender,
                                    genderCondition: genderCondition,
                                    ChkBuy: ChkBuy,
                                    buyCondition: buyCondition,
                                    buyTimes: buyTimes,
                                    buyTimeMin: buyTimeMin,
                                    buyTimeMax: buyTimeMax,
                                    ChkAge: ChkAge,
                                    ageMin: ageMin,
                                    ageMax: ageMax,
                                    ChkCancel: ChkCancel,
                                    cancelCondition: cancelCondition,
                                    cancelTimes: cancelTimes,
                                    cancelTimeMin: cancelTimeMin,
                                    cancelTimeMax: cancelTimeMax,
                                    ChkRegisterTime: ChkRegisterTime,
                                    registerTimeMin: registerTimeMin,
                                    registerTimeMax: registerTimeMax,
                                    ChkReturn: ChkReturn,
                                    returnCondition: returnCondition,
                                    returnTimes: returnTimes,
                                    returnTimeMin: returnTimeMin,
                                    returnTimeMax: returnTimeMax,
                                    ChkLastOrder: ChkLastOrder,
                                    lastOrderMin: lastOrderMin,
                                    lastOrderMax: lastOrderMax,
                                    ChkNotice: ChkNotice,
                                    noticeCondition: noticeCondition,
                                    noticeTimes: noticeTimes,
                                    ChkLastLogin: ChkLastLogin,
                                    lastLoginMin: lastLoginMin,
                                    lastLoginMax: lastLoginMax,
                                    ChkTotalConsumption: ChkTotalConsumption,
                                    totalConsumptionMin: totalConsumptionMin,
                                    totalConsumptionMax: totalConsumptionMax,
                                    ChkBlackList: ChkBlackList
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        ConditionNameStore.removeAll();
                                        Ext.getCmp("conditionName").reset();
                                        Ext.Msg.alert(INFORMATION, "保存成功!");
                                        store.load();
                                        Ext.getCmp("show").reset();
                                        editWin.close();
                                    }
                                    else {
                                        if (result.msg == '0') {
                                            Ext.Msg.alert(INFORMATION, "保存篩選條件名稱失敗");
                                        }
                                        else if (result.msg == '1') {
                                            Ext.Msg.alert(INFORMATION, "篩選條件名稱已存在！");
                                            Ext.getCmp("btn_saveName").setDisabled(false);
                                            Ext.getCmp("edmconditionName").reset();
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    }
                                },
                                failure: function (form, action) {
                                    if (Ext.decode(action.response.responseText).msg == '0') {
                                        Ext.Msg.alert(INFORMATION, "保存篩選條件名稱失敗");
                                    }
                                    else if (Ext.decode(action.response.responseText).msg == '1') {
                                        Ext.Msg.alert(INFORMATION, "篩選條件名稱已存在！");
                                        Ext.getCmp("btn_saveName").setDisabled(false);
                                        Ext.getCmp("edmconditionName").reset();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                }

                            });
                        }
                    }
                }
            }
        }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '保存條件名稱',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        layout: 'fit',
        items: [saveFrm],
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
        ]
    });
    editWin.show();
};

