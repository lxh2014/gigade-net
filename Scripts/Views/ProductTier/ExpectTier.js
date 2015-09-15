
var PRODUCT_ID, OLD_PRODUCT_ID = '';
var messages = '';
var messagesrep = '';
var messagestime = '';

//排程類型store
var pcStore = Ext.create('Ext.data.Store', {
    fields: ['id', 'name'],
    data: [
        //{ "id": "1", "name": "出貨一次" },
        //{ "id": "2", "name": "重複出貨" }]
        { "id": "1", "name": EXECUTE_ONCE },//單次執行
        { "id": "2", "name": EXECUTE_REPEAT }]// edit by wwei0216w 2015/5/20 重複執行
});


//發生於data
var runStore = Ext.create('Ext.data.Store', {
    fields: ['runtype', 'runname'],
    data: [
        { "runtype": "2D", "runname": EVERYDAY },//每日
        { "runtype": "2W", "runname": EVERYWEEK },//每週
        { "runtype": "2M", "runname": EVERYMONTH }]//每月
});

//第幾周
var onweeksStore = Ext.create('Ext.data.Store', {
    fields: ['abbr', 'name'],
    data: [
        { "abbr": "1", "name": WEEK_ONE },//第一週
        { "abbr": "2", "name": WEEK_TWO },//第二週
        { "abbr": "3", "name": WEEK_THREE },//第三週
        { "abbr": "4", "name": WEEK_FOUR },//第四週
        { "abbr": "5", "name": WEEK_LAST }//最後一周
    ]
});

//那幾天
var ondaysStore = Ext.create('Ext.data.Store', {
    fields: ['abbr', 'name'],
    data: [
        { "abbr": "1", "name": MONDAY },//星期一
        { "abbr": "2", "name": TUESDAY },//星期二
        { "abbr": "3", "name": WEDNESDAY },//星期三
        { "abbr": "4", "name": THURSDAY },//星期四
        { "abbr": "5", "name": FRIDAY },//星期五
        { "abbr": "6", "name": SATURADY },//星期六
        { "abbr": "7", "name": SUNDAY },//星期日
        //{ "abbr": "8", "name": "每天" },
        { "abbr": "9", "name": WEEKEND },//週末
        { "abbr": "10", "name": WORKDAY }//工作日
    ]
});

// h m s
var timeStore = Ext.create('Ext.data.Store', {
    fields: ['abbr', 'name'],
    data: [
        { "abbr": "1", "name": HOUR },//小時
        { "abbr": "2", "name": MINUTE },//分鐘
        { "abbr": "3", "name": SECOND }//秒
    ]
});


var gxGridStore = Ext.create('Ext.data.Store', {
    fields: ['rowid', 'schedule_id', 'relation_table', 'relation_id'],
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductTier/GetRelationTiers',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});



var gxGrid = Ext.create('Ext.grid.Panel', {
    title: PRODUCT_SCHEDULE_RELEVANCE_INFO,//商品排程關係表
    id: 'gxGrid',
    hidden: true,
    frame: false,
    store: gxGridStore,
    viewConfig: {
        enableTextSelection: true
    },
    plugins: [{ ptype: 'cellediting' }],
    columns: [
         { xtype: 'rownumberer', width: 35, align: 'center', menuDisabled: true, sortable: false },
        //{ header: '序號', dataIndex: 'rowid', menuDisabled: true, sortable: false },
        //{ header: '排程ID', dataIndex: 'schedule_id', menuDisabled: true, sortable: false },
        { header: RELEVANCE_INFO, dataIndex: 'relation_table', width: 200, menuDisabled: true, sortable: false },
        {
            header: RELEVANCE_PRODUCT_ID, dataIndex: 'relation_id', menuDisabled: true, sortable: false, flex: 1,
            editor: {
                xtype: 'textfield',
                readOnly: true
            }
        }]
});


var pcFrm = Ext.create('Ext.form.Panel', {
    id: 'pcFrm',
    layout: 'anchor',
    url: '/ProductTier/TierSetSave',
    defaults: { margin: '15 15 15 15' },
    //height: 400,
    items: [{
        xtype: 'container',//類型名稱
        layout: 'column',
        id: 'jbqk',
        items: [{
            xtype: 'combobox',
            fieldLabel: SCHEDULE_TYPE,//排程類型
            allowBlank: false,
            id: 'pc_type',
            displayField: 'name',
            valueField: 'id',
            value: '2',
            labelWidth: 75,
            store: pcStore,
            listeners: {
                change: function (chose) {
                    Ext.getCmp('mst').setText("");
                    Ext.getCmp('ms').setText("");
                    if (Ext.getCmp('pc_type').getValue() == 2) {
                        Ext.getCmp('pcFrm').down('#jzxyc').setDisabled(true);
                        Ext.getCmp('pcFrm').down('#pl').setDisabled(false);
                        //Ext.getCmp('pcFrm').down('#mtpl').setDisabled(false);
                        Ext.getCmp('pcFrm').down('#cxsj').setDisabled(false);
                    } else {
                        Ext.getCmp('pcFrm').down('#pl').setDisabled(true);
                        //Ext.getCmp('pcFrm').down('#mtpl').setDisabled(true);
                        Ext.getCmp('pcFrm').down('#cxsj').setDisabled(true);
                        Ext.getCmp('pcFrm').down('#jzxyc').setDisabled(false);
                    }
                }
            }
        }, {
            xtype: 'textfield',
            labelWidth: 75,
            allowBlank: false,
            margin: '0 0 0 20',
            id: 'schedule_name',
            name: 'schedule_name',
            fieldLabel: SCHEDULE_NAME//排程名稱
        }]
    }, {
        xtype: 'textfield',
        id: 'schedule_id_win',
        name: 'schedule_id',
        hidden: true,
        fieldLabel: SCHEDULE_CODE//排程編號
    }, {
        xtype: 'fieldset',
        title: ONLY_EXECUTE_ONCE,//僅執行一次
        layout: 'anchor',
        id: 'jzxyc',
        disabled: true,
        items: [{
            xtype: 'datefield',
            format: 'Y/m/d',
            fieldLabel: DATE,//日期
            id: 'datatime',
            allowBlank: false,
            labelWidth: 54,
            margin: '10 10 15 10',
            listeners: {
                change: function () {
                    messages = SCHEDULE_WILL_IN + Ext.Date.format(Ext.getCmp('pcFrm').down('#datatime').getValue(), 'Y/m/d') + DATE_EXECUTE + '。';
                    Ext.getCmp('pcFrm').down('#ms').setText(messages);
                }
            }
        }]
    }, {
        xtype: 'fieldset',
        title: FREQUENCY,//頻率
        layout: 'anchor',
        id: 'pl',
        items: [{
            xtype: 'combobox',
            fieldLabel: HAPPEN_TO,//發生於
            allowBlank: false,
            id: 'runwhen',
            displayField: 'runname',
            valueField: 'runtype',
            labelWidth: 54,
            margin: '10 10 15 10',
            editable: false,
            //value: '2D',
            store: runStore,
            listeners: {
                change: function () {
                    switch (Ext.getCmp('runwhen').getValue()) {
                        case '2D':
                            Ext.getCmp('pcFrm').down('#everyday').show();
                            Ext.getCmp('repeatday').allowBlank = false;

                            Ext.getCmp('pcFrm').down('#everyweeks').hide();
                            Ext.getCmp('repeatweek').allowBlank = true;
                            Ext.getCmp('weeks').allowBlank = true;

                            Ext.getCmp('pcFrm').down('#everymonths').hide();
                            Ext.getCmp('nr').allowBlank = true;
                            Ext.getCmp('cfjg').allowBlank = true;
                            Ext.getCmp('njg').allowBlank = true;
                            Ext.getCmp('dijizhou').allowBlank = true;
                            Ext.getCmp('naxietian').allowBlank = true;
                            Ext.getCmp('ncfjg').allowBlank = true;
                            break;
                        case '2W':
                            Ext.getCmp('pcFrm').down('#everyday').hide();
                            Ext.getCmp('repeatday').allowBlank = true;

                            Ext.getCmp('pcFrm').down('#everyweeks').show();
                            Ext.getCmp('repeatweek').allowBlank = false;
                            Ext.getCmp('weeks').allowBlank = false;

                            Ext.getCmp('pcFrm').down('#everymonths').hide();
                            Ext.getCmp('nr').allowBlank = true;
                            Ext.getCmp('cfjg').allowBlank = true;
                            Ext.getCmp('njg').allowBlank = true;
                            Ext.getCmp('dijizhou').allowBlank = true;
                            Ext.getCmp('naxietian').allowBlank = true;
                            Ext.getCmp('ncfjg').allowBlank = true;
                            break;
                        case '2M':
                            Ext.getCmp('pcFrm').down('#everyday').hide();
                            Ext.getCmp('repeatday').allowBlank = true;

                            Ext.getCmp('pcFrm').down('#everyweeks').hide();
                            Ext.getCmp('repeatweek').allowBlank = true;
                            Ext.getCmp('weeks').allowBlank = true;

                            Ext.getCmp('pcFrm').down('#everymonths').show();
                            Ext.getCmp('nr').allowBlank = false;
                            Ext.getCmp('cfjg').allowBlank = false;
                            Ext.getCmp('njg').allowBlank = false;
                            Ext.getCmp('dijizhou').allowBlank = false;
                            Ext.getCmp('naxietian').allowBlank = false;
                            Ext.getCmp('ncfjg').allowBlank = false;
                            break;
                    }
                    messages = SCHEDULE_WILL_IN + Ext.getCmp('runwhen').getRawValue() + EXECUTE + '。';
                    Ext.getCmp('ms').setText(messages);
                }
            }
        }, {
            xtype: 'container',//每日
            layout: 'column',
            id: 'everyday',
            //hidden: true,
            items: [{
                xtype: 'numberfield',
                fieldLabel: REPEAT_FREQUENCY,//重複頻率
                allowBlank: true,
                labelWidth: 64,
                width: 136,
                minValue: 1,
                id: 'repeatday',
                listeners: {
                    change: function () {
                        switch (Ext.getCmp('runwhen').getValue()) {
                            case '2D':
                                messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('repeatday').getValue() + DAY_REPEAT_EXECUTE + '。';
                                Ext.getCmp('ms').setText(messages);
                                break;
                        }
                    }
                }
            }, {
                xtype: 'label',
                text: DAY,//天
                id: 'days',
                margin: '0 0 0 5'
            }]
        }, {
            xtype: 'container',//每週
            layout: 'column',
            id: 'everyweeks',
            hidden: true,
            items: [{
                xtype: 'numberfield',
                fieldLabel: REPEAT_FREQUENCY,//重複頻率
                allowBlank: true,
                labelWidth: 64,
                width: 136,
                minValue: 1,
                id: 'repeatweek',
                listeners: {
                    change: function () {
                        var ws = "";
                        var weeks = Ext.getCmp('weeks').getChecked()
                        for (var i = 0; i < weeks.length; i++) {
                            ws += weeks[i].boxLabel + "、";
                        }
                        ws = ws.substring(0, ws.length - 1);
                        switch (Ext.getCmp('runwhen').getValue()) {
                            case '2W':
                                messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('repeatweek').getValue() + WEEK_A + ws + EXECUTE_REPEAT + '。';
                                Ext.getCmp('ms').setText(messages);
                                break;
                        }
                    }
                }
            }, {
                xtype: 'label',
                text: WEEK_A,//週的
                margin: '0 0 0 5'
            }, {
                xtype: 'checkboxgroup',
                id: 'weeks',
                width: 500,
                margin: '0 0 0 10',
                vertical: true,
                allowBlank: true,
                items: [
                    { boxLabel: MONDAY, name: 'weekens', inputValue: '1' },
                    { boxLabel: TUESDAY, name: 'weekens', inputValue: '2'/*, checked: true */ },
                    { boxLabel: WEDNESDAY, name: 'weekens', inputValue: '3' },
                    { boxLabel: THURSDAY, name: 'weekens', inputValue: '4' },
                    { boxLabel: FRIDAY, name: 'weekens', inputValue: '5' },
                    { boxLabel: SATURADY, name: 'weekens', inputValue: '6' },
                    { boxLabel: SUNDAY, name: 'weekens', inputValue: '7' }
                ],
                listeners: {
                    change: function () {
                        var ws = "";
                        var weeks = Ext.getCmp('weeks').getChecked()
                        for (var i = 0; i < weeks.length; i++) {
                            ws += weeks[i].boxLabel + "、";
                        }
                        ws = ws.substring(0, ws.length - 1);
                        switch (Ext.getCmp('runwhen').getValue()) {
                            case '2W':
                                messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('repeatweek').getValue() + WEEK_A + ws + EXECUTE_REPEAT + '。';
                                Ext.getCmp('ms').setText(messages);
                                break;
                        }
                    }
                }
            }]
        }, {
            xtype: 'container',//每月
            id: 'everymonths',
            layout: 'anchor',
            hidden: true,
            items: [{
                xtype: 'container',
                layout: 'column',
                id: 'riqi',
                items: [{
                    xtype: 'radiofield',
                    boxLabel: DATE,//日期
                    width: 69,
                    name: 'on',
                    checked: true,
                    inputValue: 'd',
                    id: 'riqiradio',
                    listeners: {
                        change: function (chack) {
                            if (chack.checked) {
                                Ext.getCmp('nr').setDisabled(false);
                                Ext.getCmp('ri').setDisabled(false);
                                Ext.getCmp('cfjg').setDisabled(false);
                                Ext.getCmp('myyc').setDisabled(false);
                                Ext.getCmp('njg').setDisabled(false);
                                Ext.getCmp('myyc01').setDisabled(false);
                                Ext.getCmp('dijizhou').setDisabled(true);
                                Ext.getCmp('naxietian').setDisabled(true);
                                Ext.getCmp('cfjgm').setDisabled(true);
                                Ext.getCmp('ncfjg').setDisabled(true);
                                Ext.getCmp('myyc').setDisabled(true);
                            } else {
                                Ext.getCmp('nr').setDisabled(true);
                                Ext.getCmp('ri').setDisabled(true);
                                Ext.getCmp('cfjg').setDisabled(true);
                                Ext.getCmp('myyc').setDisabled(true);
                                Ext.getCmp('njg').setDisabled(true);
                                Ext.getCmp('myyc01').setDisabled(true);
                                Ext.getCmp('dijizhou').setDisabled(false);
                                Ext.getCmp('naxietian').setDisabled(false);
                                Ext.getCmp('cfjgm').setDisabled(false);
                                Ext.getCmp('ncfjg').setDisabled(false);
                                Ext.getCmp('myyc').setDisabled(false);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    width: 67,
                    minValue: 1,
                    maxValue: 31,
                    id: 'nr',
                    listeners: {
                        change: function () {
                            switch (Ext.getCmp('runwhen').getValue()) {
                                case '2M':
                                    messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('njg').getValue() + MONTH_A + Ext.getCmp('nr').getValue() + DAY_REPEAT_EXECUTE + '。';
                                    Ext.getCmp('ms').setText(messages);
                                    break;
                            }
                        }
                    }
                }, {
                    xtype: 'label',
                    text: DAY_SUN,//日
                    id: 'ri',
                    margin: '0 0 0 5'
                }, {
                    xtype: 'label',
                    text: REPEAT_INTERVAL_EVERY,//重複間隔每
                    id: 'cfjg',
                    margin: '0 0 0 20'
                }, {
                    xtype: 'numberfield',
                    width: 67,
                    minValue: 1,
                    maxValue: 12,
                    margin: '0 0 0 5',
                    id: 'njg',
                    listeners: {
                        change: function () {
                            switch (Ext.getCmp('runwhen').getValue()) {
                                case '2M':
                                    messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('njg').getValue() + MONTH_A + Ext.getCmp('nr').getValue() + DAY_REPEAT_EXECUTE + '。';
                                    Ext.getCmp('ms').setText(messages);
                                    break;
                            }
                        }
                    }
                }, {
                    xtype: 'label',
                    text: A_MONTH_ONCE,//個月一次
                    id: 'myyc01',
                    margin: '0 0 0 5'
                }]
            }, {
                xtype: 'container',
                layout: 'column',
                id: 'zai',
                margin: '5 0 0 0',
                items: [{
                    xtype: 'radiofield',
                    boxLabel: STAY_IN,//在
                    width: 69,
                    name: 'on',
                    inputValue: 'z',
                    id: 'zairadio'
                }, {
                    xtype: 'combobox',
                    allowBlank: true,
                    id: 'dijizhou',
                    displayField: 'name',
                    valueField: 'abbr',
                    width: 100,
                    editable: false,
                    disabled: true,
                    value: 1,
                    store: onweeksStore,
                    listeners: {
                        change: function () {
                            switch (Ext.getCmp('runwhen').getValue()) {
                                case '2M':
                                    messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('ncfjg').getValue() + MONTH_A + Ext.getCmp('dijizhou').getRawValue() + DE + Ext.getCmp('naxietian').getRawValue() + DAY_REPEAT_EXECUTE + '。';
                                    Ext.getCmp('ms').setText(messages);
                                    break;
                            }
                        }
                    }
                }, {
                    xtype: 'combobox',
                    allowBlank: true,
                    disabled: true,
                    id: 'naxietian',
                    displayField: 'name',
                    valueField: 'abbr',
                    width: 100,
                    editable: false,
                    value: 1,
                    margin: '0 0 0 10',
                    store: ondaysStore,
                    listeners: {
                        change: function () {
                            switch (Ext.getCmp('runwhen').getValue()) {
                                case '2M':
                                    messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('ncfjg').getValue() + MONTH_A + Ext.getCmp('dijizhou').getRawValue() + DE + Ext.getCmp('naxietian').getRawValue() + DAY_REPEAT_EXECUTE + '。';
                                    Ext.getCmp('ms').setText(messages);
                                    break;
                            }
                        }
                    }
                }, {
                    xtype: 'label',
                    text: REPEAT_INTERVAL_EVERY,//重複間隔每
                    id: 'cfjgm',
                    disabled: true,
                    margin: '0 0 0 20'
                }, {
                    xtype: 'numberfield',
                    width: 67,
                    minValue: 1,
                    maxValue: 12,
                    margin: '0 0 0 5',
                    disabled: true,
                    id: 'ncfjg',
                    listeners: {
                        change: function () {
                            switch (Ext.getCmp('runwhen').getValue()) {
                                case '2M':
                                    messages = SCHEDULE_WILL_IN_EVERY + Ext.getCmp('ncfjg').getValue() + MONTH_A + Ext.getCmp('dijizhou').getRawValue() + DE + Ext.getCmp('naxietian').getRawValue() + EXECUTE_REPEAT + '。';
                                    Ext.getCmp('ms').setText(messages);
                                    break;
                            }
                        }
                    }
                }, {
                    xtype: 'label',
                    text: A_MONTH_ONCE,//個月一次
                    id: 'myyc',
                    disabled: true,
                    margin: '0 0 0 5'
                }]
            }]
        }]
    },
    /* {
            xtype: 'fieldset',
            title: '每日頻率',
            id: 'mtpl',
            layout: 'anchor',
            padding: '8 8 8 8',
            items: [{
                xtype: 'container',
                layout: 'column',
                id: 'zhixingyici',
                items: [{
                    xtype: 'radiofield',
                    boxLabel: '執行一次於',
                    width: 100,
                    name: 'in',
                    inputValue: 'h',
                    id: 'zxycradio',
                    checked: true,
                    listeners: {
                        change: function (chack) {
                            if (chack.checked) {
                                Ext.getCmp('ro_hou').setDisabled(false);
                                Ext.getCmp('ro_min').setDisabled(false);
                                Ext.getCmp('ro_sen').setDisabled(false);
                                Ext.getCmp('cfnh').setDisabled(true);
                                Ext.getCmp('cfhms').setDisabled(true);
                                Ext.getCmp('s_hour').setDisabled(true);
                                Ext.getCmp('s_min').setDisabled(true);
                                Ext.getCmp('s_sen').setDisabled(true);
                                Ext.getCmp('e_hour').setDisabled(true);
                                Ext.getCmp('e_min').setDisabled(true);
                                Ext.getCmp('e_sen').setDisabled(true);

                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);

                            } else {
                                Ext.getCmp('ro_hou').setDisabled(true);
                                Ext.getCmp('ro_min').setDisabled(true);
                                Ext.getCmp('ro_sen').setDisabled(true);
                                Ext.getCmp('cfnh').setDisabled(false);
                                Ext.getCmp('cfhms').setDisabled(false);
                                Ext.getCmp('s_hour').setDisabled(false);
                                Ext.getCmp('s_min').setDisabled(false);
                                Ext.getCmp('s_sen').setDisabled(false);
                                Ext.getCmp('e_hour').setDisabled(false);
                                Ext.getCmp('e_min').setDisabled(false);
                                Ext.getCmp('e_sen').setDisabled(false);

                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);

                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    width: 43,
                    minValue: 00,
                    maxValue: 23,
                    margin: '0 0 0 1',
                    id: 'ro_hou',
                    listeners: {
                        change: function () {
                            if (!Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    id: 'ro_min',
                    listeners: {
                        change: function () {
                            if (!Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    id: 'ro_sen',
                    listeners: {
                        change: function () {
                            if (!Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }]
            }, {
                xtype: 'container',
                layout: 'column',
                id: 'cyzxy',
                margin: '10 0 0 0',
                items: [{
                    xtype: 'radiofield',
                    boxLabel: '重複執行於每',
                    width: 100,
                    name: 'in',
                    inputValue: 'j',
                    id: 'cfzxradio'
                }, {
                    xtype: 'numberfield',
                    width: 67,
                    minValue: 1,
                    maxValue: 59,
                    disabled: true,
                    id: 'cfnh',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'combobox',
                    allowBlank: false,
                    id: 'cfhms',
                    disabled: true,
                    displayField: 'name',
                    valueField: 'abbr',
                    width: 60,
                    margin: '0 0 0 15',
                    editable: false,
                    value: 1,
                    store: timeStore,
                    listeners: {
                        select: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: '開始時間',
                    labelWidth: 64,
                    width: 110,
                    minValue: 00,
                    maxValue: 23,
                    margin: '0 0 0 15',
                    disabled: true,
                    id: 's_hour',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    disabled: true,
                    id: 's_min',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    disabled: true,
                    id: 's_sen',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: '結束時間',
                    labelWidth: 64,
                    width: 110,
                    minValue: 00,
                    maxValue: 23,
                    margin: '0 0 0 15',
                    disabled: true,
                    id: 'e_hour',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    disabled: true,
                    id: 'e_min',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }, {
                    xtype: 'numberfield',
                    fieldLabel: ' ',
                    labelWidth: 3,
                    width: 50,
                    minValue: 00,
                    maxValue: 59,
                    margin: '0 0 0 1',
                    disabled: true,
                    id: 'e_sen',
                    listeners: {
                        change: function () {
                            if (Ext.getCmp('noendtime').checked) {
                                messagesrep = '于當天' + Ext.getCmp('ro_hou').getValue() + ':' + Ext.getCmp('ro_min').getValue() + ':' + Ext.getCmp('ro_sen').getValue() + '執行一次。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            } else {
                                messagesrep = '于當天' + Ext.getCmp('s_hour').getValue() + ':' + Ext.getCmp('s_min').getValue() + ':' + Ext.getCmp('s_sen').getValue() + ' ~ ' + Ext.getCmp('e_hour').getValue() + ':' + Ext.getCmp('e_min').getValue() + ':' + Ext.getCmp('e_sen').getValue() + '內，每' + Ext.getCmp('cfnh').getValue() + Ext.getCmp('cfhms').getRawValue() + '重複執行。';
                                Ext.getCmp('mscf').setText(messagesrep);
                            }
                        }
                    }
                }]
            }]
        },*/
       {
           xtype: 'fieldset',
           title: CONTINUE_TIME,//持續時間
           id: 'cxsj',
           layout: 'column',
           padding: '10 10 10 10',
           items: [{
               xtype: 'datefield',
               format: 'Y/m/d',
               fieldLabel: BEGIN_TIME,//開始時間
               allowBlank: false,
               labelWidth: 64,
               width: 220,
               margin: '0 0 0 15',
               id: 'cs_time',
               listeners: {
                   change: function () {
                       if (Ext.getCmp('noendtime').checked) {
                           messagestime = BEGIN_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d ') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       } else {
                           messagestime = EXECUTE_TIME_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d ') + ' ~ ' + Ext.Date.format(Ext.getCmp('ce_time').getValue(), 'Y/m/d') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       }
                   }
               }
           }, {
               xtype: 'datefield',
               format: 'Y/m/d',
               fieldLabel: END_TIME,//結束時間
               allowBlank: false,
               labelWidth: 64,
               width: 220,
               margin: '0 0 0 20',
               id: 'ce_time',
               listeners: {
                   change: function () {
                       if (Ext.getCmp('noendtime').checked) {
                           messagestime = BEGIN_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       } else {
                           messagestime = EXECUTE_TIME_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d ') + ' ~ ' + Ext.Date.format(Ext.getCmp('ce_time').getValue(), 'Y/m/d ') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       }
                   }
               }
           }, {
               xtype: 'checkbox',
               boxLabel: NON_END_TIME,//沒有結束時間
               width: 100,
               inputValue: 'noentime',
               id: 'noendtime',
               margin: '0 0 0 20',
               listeners: {
                   change: function (chack) {
                       if (chack.checked) {
                           Ext.getCmp('ce_time').setDisabled(true);
                           messagestime = BEGIN_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d ') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       } else {
                           Ext.getCmp('ce_time').setDisabled(false);
                           messagestime = EXECUTE_TIME_ON + '：' + Ext.Date.format(Ext.getCmp('cs_time').getValue(), 'Y/m/d') + ' ~ ' + Ext.Date.format(Ext.getCmp('ce_time').getValue(), 'Y/m/d') + '。';
                           Ext.getCmp('mst').setText(messagestime);
                       }
                   }
               }
           }]
       }, {
           xtype: 'container',
           layout: 'column',
           id: 'wenben',
           height: 19,
           items: [{
               xtype: 'label',
               id: 'ms',
               text: messages
           },
              //{
              //    xtype: 'label',
              //    id: 'mscf',
              //    text: messagesrep
              //},
             {
                 xtype: 'label',
                 id: 'mst',
                 text: messagestime
             }]
       }],
    buttons: [{
        text: SAVE,//保存
        id: 'btnSave',
        //formBind: true,
        //disabled: true,
        handler: function () {
            var form = this.up('form').getForm();
            if (form.isValid()) {
                form.submit({
                    params: getParams(),
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            addPc.hide();
                            tierStore.load();
                            Ext.Msg.alert(INFORMATION, SAVE_SUCCESS);
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



var addPc = Ext.create('Ext.window.Window', {
    title: SCHEDULE_CONFIG,//排成設定
    width: 800,
    //height: 450,
    layout: 'anchor',
    items: [pcFrm],
    closeAction: 'hide',
    modal: true,
    resizable: true,
    autoScroll: true,
    labelWidth: 60,
    padding: '5 5 5 5'
});

function Tier_Load(record) {
    Ext.getCmp('schedule_id_win').setValue(record.data.schedule_id);
    Ext.getCmp('schedule_name').setValue(record.data.schedule_name);

    switch (record.data.type) {
        case 2:
            Ext.getCmp('pc_type').setValue(2)
            var runwhen = record.data.execute_type;
            switch (runwhen) {

                case '2D':
                    Ext.getCmp('runwhen').setValue(runwhen)
                    Ext.getCmp('repeatday').setValue(record.data.repeat_count);
                    break;

                case '2W':
                    Ext.getCmp('runwhen').setValue(runwhen);
                    Ext.getCmp('repeatweek').setValue(record.data.repeat_count);
                    Ext.getCmp("weeks").setValue({
                        'weekens': record.data.week_day.split(',')
                    });
                    break;

                case '2M':
                    Ext.getCmp('runwhen').setValue(runwhen);
                    if (record.data.month_type == "1") {
                        Ext.getCmp('riqiradio').setValue(true);
                        Ext.getCmp('nr').setValue(record.data.date_value);
                        Ext.getCmp('njg').setValue(record.data.repeat_count);
                    } else {
                        Ext.getCmp('zairadio').setValue(true);
                        Ext.getCmp('dijizhou').setValue(record.data.date_value);
                        Ext.getCmp('naxietian').setValue(record.data.week_day);
                        Ext.getCmp('ncfjg').setValue(record.data.repeat_count);
                    }
                    break;
            }

            if (record.data.day_type == "1") {
                Ext.getCmp('zxycradio').setValue(true);
                var rotimes = record.data.start_time.split(':');
                Ext.getCmp('ro_hou').setValue(rotimes[0]);
                Ext.getCmp('ro_min').setValue(rotimes[1]);
                Ext.getCmp('ro_sen').setValue(rotimes[2]);
            } /*else {
                Ext.getCmp('cfzxradio').setValue(true);
                Ext.getCmp('cfnh').setValue(record.data.repeat_hours);
                Ext.getCmp('cfhms').setValue(record.data.time_type);
                var simes = record.data.start_time.split(':');
                Ext.getCmp('s_hour').setValue(simes[0]);
                Ext.getCmp('s_min').setValue(simes[1]);
                Ext.getCmp('s_sen').setValue(simes[2]);
                var eimes = record.data.end_time.split(':');
                Ext.getCmp('e_hour').setValue(eimes[0]);
                Ext.getCmp('e_min').setValue(eimes[1]);
                Ext.getCmp('e_sen').setValue(eimes[2]);
            }*/

            Ext.getCmp('cs_time').setRawValue(record.data.duration_start);

            if (record.data.duration_end == "0001/01/01" || record.data.duration_end == "") {
                Ext.getCmp('noendtime').setValue(true);
                Ext.getCmp('ce_time').setRawValue("");
            } else {
                Ext.getCmp('noendtime').setValue(false);
                Ext.getCmp('ce_time').setRawValue(record.data.duration_end);
            }
            break;

        case 1:
            Ext.getCmp('pc_type').setValue(1)
            Ext.getCmp('datatime').setRawValue(record.data.duration_start);
            break;
    }

    var desces = record.data.desc.split("。");
    var ms = desces[0] + '。';
    if (desces[1] == "") {
        var mst = desces[1];
    } else {
        var mst = desces[1] + '。';
    }
    Ext.getCmp('ms').setText(ms);
    Ext.getCmp('mst').setText(mst);

}


function getParams() {
    var params = new Object();
    var pc_type = params['type'] = Ext.getCmp('pc_type').getValue();
    if (Ext.getCmp('schedule_id_win').getValue() != "") {
        params.schedule_id = Ext.getCmp('schedule_id_win').getValue();
    } else {
        params.schedule_id = 0;
    }

    params.schedule_name = Ext.getCmp('schedule_name').getValue();
    params.desc = Ext.getCmp('ms').text + Ext.getCmp('mst').text;

    switch (pc_type) {
        case '2':
            var runwhen = params.execute_type = Ext.getCmp('runwhen').getValue();
            switch (runwhen) {
                case '2D':
                    params.repeat_count = Ext.getCmp('repeatday').getValue();
                    break;
                case '2W':
                    params.repeat_count = Ext.getCmp('repeatweek').getValue();
                    var week = Ext.getCmp('weeks').getChecked();
                    var weekStr = "";
                    for (var i = 0; i <= week.length - 1; i++) {
                        weekStr += week[i].inputValue + ",";
                    }
                    params.week_day = weekStr.substring(0, weekStr.length - 1);
                    break;
                case '2M':
                    if (Ext.getCmp('riqiradio').checked) {
                        params.month_type = 1;
                        params.date_value = Ext.getCmp('nr').getValue();
                        params.repeat_count = Ext.getCmp('njg').getValue();
                    } else {
                        params.month_type = 2;
                        params.date_value = Ext.getCmp('dijizhou').getValue();
                        params.week_day = Ext.getCmp('naxietian').getValue();
                        params.repeat_count = Ext.getCmp('ncfjg').getValue();
                    }
                    break;
            }
            //var zxycradio = Ext.getCmp('zxycradio');
            //if (zxycradio.checked) {
            //    params.day_type = 1;
            //    params.start_time = Ext.String.format('{0}:{1}:{2}',
            //        Ext.getCmp('ro_hou').getValue(),
            //        Ext.getCmp('ro_min').getValue(),
            //        Ext.getCmp('ro_sen').getValue());
            //} else {
            //    params.day_type = 2;
            //    params.repeat_hours = Ext.getCmp('cfnh').getValue();
            //    params.time_type = Ext.getCmp('cfhms').getValue();
            //    params.start_time = Ext.String.format('{0}:{1}:{2}',
            //        Ext.getCmp('s_hour').getValue(),
            //        Ext.getCmp('s_min').getValue(),
            //        Ext.getCmp('s_sen').getValue());
            //    params.end_time = Ext.String.format('{0}:{1}:{2}',
            //        Ext.getCmp('e_hour').getValue(),
            //        Ext.getCmp('e_min').getValue(),
            //        Ext.getCmp('e_sen').getValue());
            //}
            params.duration_start = Ext.getCmp('cs_time').getValue();

            if (!Ext.getCmp('noendtime').checked) {
                params.duration_end = Ext.getCmp('ce_time').getValue();
            }
            break;
        case '1':
            params.duration_start = Ext.getCmp('datatime').getValue();
            break;
    }

    return params;
}