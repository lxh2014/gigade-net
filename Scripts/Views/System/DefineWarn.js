var d_rowIds = 0;

/********* 引用文件 **********/

Ext.Loader.setPath('Ext.app', 'classes');

Ext.require([
            'Ext.layout.container.*',
            'Ext.resizer.Splitter',
            'Ext.fx.target.Element',
            'Ext.fx.target.Component',
            'Ext.window.Window',
            'Ext.app.PortalColumn',
            'Ext.app.PortalPanel',
]);


/********自定义保存按鈕*********/
Ext.define('GIGADE.BtnSave', {
    extend: 'Ext.panel.Tool',
    type: 'save',
    width: 20,
    name: 'btnSave',
    disabled: true
});


/*********自定义Panel**********/
Ext.define('GIGADE.DraPanel', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.DraPanel',
    id: '',
    layout: 'fit',
    margin: '0 5 8 0',
    padding: '0 0 10 0',
    frame: true,
    collapsible: true,
    animCollapse: true,
    draggable: true,
    listeners: {
        //        collapse: function (panel) {
        //            panel.query('*[name=btnSave]')[0].setDisabled(true);
        //        },
        //        expand: function (panel) {
        //            panel.query('*[name=btnSave]')[0].setDisabled(false);
        //        }
    }
});

//收件人model
Ext.define('GIGADE.RECEIVER', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'callid', type: "int" },
        { name: 'user_mail', type: "string" },
        { name: 'user_name', type: "string" },
        { name: 'row_id', type: "int" },
        { name: 'group_name', type: "string" },
        { name: 'update_user', type: "string" },
        { name: 'group_code', type: "string" }
    ]
});

var receiverStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.RECEIVER',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/MailGroupList?IsPage=false',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});


var sendStore = Ext.create('Ext.data.Store', {
    fields: ['group_name', 'user_mail', 'row_id', 'group_id'],
    idProperty: 'row_id',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/GetMemberInfo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var sendStore2 = Ext.create('Ext.data.Store', {
    fields: ['group_name', 'user_mail', 'row_id', 'group_id'],
    idProperty: 'row_id',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/GetMemberInfo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var sendStore3 = Ext.create('Ext.data.Store', {
    fields: ['group_name', 'user_mail', 'row_id', 'group_id'],
    idProperty: 'row_id',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/GetMemberInfo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//var AuthView = Ext.create('Ext.view.View', {
//    deferInitialRefresh: false,
//    autoScroll: true,
//    frame: true,
//    plain: true,
//    store: sendStore,
//    tpl: Ext.create('Ext.XTemplate',
//        '<div id="AuthView" class="View">',
//            //'<tpl for=".">',
//                '<div class="group" boder:"1px">',
//                    //'<h2><div>{groupName}</div></h2>',
//                    '<dl>',
//                        '<tpl for="item">',
//                            '<div name = "div_user" class="name downline">',
//                                '<label>{user_name}</label>&nbsp;<lable name="{row_id}" onclick="pageCancel(this)" ">X</lable><br/>',
//                            '</div>',
//                        '</tpl>',
//                    '</dl>',
//                '</div>',
//            //'</tpl>',
//        '</div>'
//    ),
//    itemSelector: 'dl',
//    overItemCls: 'group-hover'
//});


//收件人組
var receiverCombox = Ext.create('Ext.form.ComboBox', {
    store: receiverStore,
    name: 'receiverComb',
    queryMode: 'local',
    displayField: 'group_name',
    valueField: 'row_id'
});

//收件人控件
var receiverControl = {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    labelWidth: 65,
    fieldLabel: '收件人組',
    items: [receiverCombox, {
        xtype: 'button',
        name: 'Enter',
        text: '確認',
        width: 80,
        margin: '0 0 0 7',
        listeners: {
            click: function (btn) {
                var panel = btn.up('panel');
                var rowId = panel.query('*[name=receiverComb]')[0].value;
                if (panel.container.id.indexOf('stockFrm') != -1) {
                    var oldItem = sendStore.data.items //需要被添加的數據
                    sendStore.load({
                        params: { 'rowId': rowId },
                        callback: function (records, operation, success) {
                            var a = 0;
                            if (success == true && records.length != 0) {
                                if (findModel(oldItem, records) == true) {
                                    sendStore.removeAll();
                                }
                            }
                            sendStore.add(oldItem);
                        }
                    });
                } else if (panel.container.id.indexOf('productFrm') != -1) {
                    var oldItem = sendStore2.data.items //需要被添加的數據
                    sendStore2.load({
                        params: { 'rowId': rowId },
                        callback: function (records, operation, success) {
                            var a = 0;
                            if (success == true && records.length != 0) {
                                if (findModel(oldItem, records) == true) {
                                    sendStore2.removeAll();
                                }
                            }
                            sendStore2.add(oldItem);
                        }
                    });
                } else if (panel.container.id.indexOf('productMapFrm') != -1) {
                    var oldItem = sendStore3.data.items //需要被添加的數據
                    sendStore3.load({
                        params: { 'rowId': rowId },
                        callback: function (records, operation, success) {
                            var a = 0;
                            if (success == true && records.length != 0) {
                                if (findModel(oldItem, records) == true) {
                                    sendStore3.removeAll();
                                }
                            }
                            sendStore3.add(oldItem);
                        }
                    });
                }
            }
        }
    }]
};



Ext.apply(Ext.form.field.VTypes, {
    VHour: function (v) {
        return /^(((0|1)?[0-9])|(2[0-3]))$/.test(v);
    },
    VHourText: FORMAT_ERROR
});

Ext.apply(Ext.form.field.VTypes, {
    VMinute: function (v) {
        return /^(((0|1|2|3|4|5)?[0-9]))$/.test(v);
    },
    VMinuteText: FORMAT_ERROR
});

function pageCancel(object, panel) {
    var id = object.attributes[1].nodeValue;
    var panelId = panel.id;
    switch (panelId) {
        case "stockFrm":
            sendStore.removeAt(sendStore.find("row_id", id));
            break;
        case "productFrm":
            sendStore2.removeAt(sendStore2.find("row_id", id));
            break;
        case "productMapFrm":
            sendStore3.removeAt(sendStore3.find("row_id", id));
            break;
    }
    panel.doLayout();
}




/***********创建Item(公用)**********/
var items = [{
    xtype: 'hidden',
    name: 'switchId',
    value: 0
}, {
    xtype: 'checkbox',
    boxLabel: MAIL_SWITCH,
    name: 'switch',
    margin: '0 0 20 0'
}, {
    xtype: 'fieldcontainer',
    layout: 'hbox',
    labelWidth: 65,
    fieldLabel: SEND_TIME,
    items: [{
        xtype: 'textfield',
        width: 100,
        vtype: 'VHour',
        name: 'SendHour',
        fieldStyle: { textAlign: 'center' },
        emptyText: TIME_HOUR
    }, {
        xtype: 'displayfield',
        padding: '0 10',
        value: ':'
    }, {
        xtype: 'textfield',
        width: 100,
        vtype: 'VMinute',
        name: 'SendMinute',
        fieldStyle: { textAlign: 'center' },
        emptyText: TIME_MINUTE,
        listeners: {
            blur: function (num) {
                var val = this.value;
                if (!isNaN(val) && val.length < 2) {
                    this.setValue('0' + val);
                }
            }
        }
    }, {
        xtype: 'button',
        text: BTN_ADD,
        margin: '0 0 0 10',
        width: 80,
        handler: function (btn) {
            var thisPanel = btn.up('panel');
            var id = thisPanel.query('*[name=SendHour]')[0].id;
            var hour = thisPanel.query('*[name=SendHour]')[0];
            var minute = thisPanel.query('*[name=SendMinute]')[0];
            var hourValue = hour.getValue();
            var minuValue = minute.getValue();

            if (hourValue == '' || minuValue == '') {
                hour.markInvalid(INPUT_PLEASE + TIME_HOUR);
                minute.markInvalid(INPUT_PLEASE + TIME_MINUTE);
                return;
            }

            var areaTime = thisPanel.query('*[name=sendTime]')[0];
            var areaValue = areaTime.getValue();
            var newValue = hourValue + ':' + minuValue;
            var values = areaValue.split(',');
            for (var i = 0, j = values.length; i < j; i++) {
                if (values[i] == newValue) {
                    areaTime.markInvalid(TIME_EXIST);
                    return;
                }
            }

            if (areaValue != '') {
                areaValue += ',';
            }

            areaTime.setValue(areaValue + newValue);


        }
    }, {
        xtype: 'button',
        text: BTN_CLEAR,
        name: 'btnClear',
        width: 80,
        margin: '0 0 0 7',
        handler: function (btn) {
            var thisPanel = btn.up('panel');
            var areaTime = thisPanel.query('*[name=sendTime]')[0];
            areaTime.setValue('');
        }
    }]
}, {
    xtype: 'hidden',
    name: 'sendTimeId',
    value: 0
}, {
    xtype: 'textarea',
    fieldLabel: TIME_ARRAY,
    name: 'sendTime',
    readOnly: true,
    labelWidth: 65,
    allowBlank: false

}, {
    xtype: 'hidden',
    name: 'sendToId',
    value: 0
},
 receiverControl
    //{
    //    xtype: 'textarea',
    //    fieldLabel: SEND_TO,
    //    emptyText: MANY_SPLIT,
    //    labelWidth: 65,
    //    name: 'sendTo',
    //    height: 130,
    //    allowBlank: false
    //}
];


function findModel(items, records) {
    var flag = false;
    var name = records[0].data.group_name
    for (var i = 0; i < items.length; i++) {
        if (items[i].data.group_name == name) {
            flag = true;
            break;
        }
    }
    return flag
}




