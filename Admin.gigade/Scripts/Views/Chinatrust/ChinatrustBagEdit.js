Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: ''
});
//下拉框
Ext.define("gigade.BagModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "event_id", type: "string" },
       { name: "event_name", type: "string" },
        
    ]
});
var BagStore = Ext.create('Ext.data.Store', {
    model: 'gigade.BagModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Chinatrust/GetChinaTrustStore",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Chinatrust/EventChinaTrustBagSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '區域包編號',
                id: 'bag_id',
                name: 'bag_id',
                hidden:true,
            },
            {
                xtype: 'combobox',
                fieldLabel: '活動名稱',
                id: 'event_id',
                name: 'event_id',
                valueField: 'event_id',
                displayField: 'event_name',
                editable: false,
                queryModel:'remote',
                lastQuery:'',
                store: BagStore,
                emptyText: '請選擇...',
                allowBlank:false,
                value:document.getElementById('eventId').value
            },
            {
                xtype: 'textfield',
                fieldLabel: '區域包名稱',
                id: 'bag_name',
                name: 'bag_name',
                maxLength:10,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '區域包描述',
                id: 'bag_desc',
                name: 'bag_desc',
            },
            {
                xtype: 'filefield',
                fieldLabel: '圖片',
                id: 'bag_banner',
                name: 'bag_banner',
                buttonText: '選擇圖片...',
                validator:
                          function (value) {
                              if (value != '') {
                                  var type = value.split('.');
                                  var extention = type[type.length - 1].toString().toLowerCase();
                                  if (extention == 'gif' || extention == 'png' || extention == 'jpg') {
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
                submitValue: true,
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '活動開始時間',
                id: 'bag_start_time',
                name: 'bag_start_time',
                format: 'Y-m-d H:i:s',
                editable: false,
                allowBlank: false,
                value: Tomorrow(),
               time: { hour: 00, min: 00, sec: 00 },
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("bag_start_time");
                       var end = Ext.getCmp("bag_end_time");
                       if (end.getValue() < start.getValue()) {
                           var start_date = start.getValue();
                           Ext.getCmp('bag_end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                       }
                   }
               }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '活動結束時間',
                id: 'bag_end_time',
                name: 'bag_end_time',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
               time: { hour: 23, min: 59, sec:59 },
                editable: false,
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("bag_start_time");
                        var end = Ext.getCmp("bag_end_time");
                        if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                            var end_date = end.getValue();
                            Ext.getCmp('bag_start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                        }
                    }
                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '顯示時間開始',
                id: 'bag_show_start_time',
                name: 'bag_show_start_time',
                time: { hour: 00, min: 00, sec: 00 },
                format: 'Y-m-d H:i:s',
                editable: false,
                allowBlank: false,
                value: Tomorrow(),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("bag_show_start_time");
                        var end = Ext.getCmp("bag_show_end_time");
                        if (end.getValue() < start.getValue()) {
                            var start_date = start.getValue();
                            Ext.getCmp('bag_show_end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                        }
                    }
                }
            },
            {
                xtype: 'datetimefield',
                fieldLabel: '顯示時間結束',
                id: 'bag_show_end_time',
                name: 'bag_show_end_time',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                editable: false,
                time: { hour: 23, min: 59, sec:59 },
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("bag_show_start_time");
                        var end = Ext.getCmp("bag_show_end_time");
                        if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                            var end_date = end.getValue();
                            Ext.getCmp('bag_show_start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                        }
                    }
                }
            },
            {
                fieldLabel:'商品數量',
                xtype: 'numberfield',
                id: 'product_number',
                name: 'product_number',
                minValue: 0,
                value:0,
                allowDecimals:false,
            },
            {
                xtype: 'displayfield',
                fieldLabel: '創建人',
                id: 'create_user',
                name: 'create_user',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '修改人',
                id: 'update_user',
                name: 'update_user',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '創建時間',
                id: 'create_time',
                name: 'create_time',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '更新時間',
                id: 'update_time',
                name: 'update_time',
                hidden: true
            }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (Ext.getCmp('bag_start_time').getValue() > Ext.getCmp('bag_end_time').getValue())
                    {
                        Ext.Msg.alert("提示信息", "活動開始時間不能大於活動結束時間！");
                        return;
                    }
                    if (Ext.getCmp('bag_show_start_time').getValue() > Ext.getCmp('bag_show_end_time').getValue()) {
                        Ext.Msg.alert("提示信息", "顯示時間開始不能大於顯示時間開始結束！");
                        return;
                    }
                        if (form.isValid()) {
                            form.submit({
                                params: {
                                    bag_id: Ext.htmlEncode(Ext.getCmp('bag_id').getValue()),
                                    event_id: Ext.htmlEncode(Ext.getCmp('event_id').getValue()),
                                    bag_name: Ext.htmlEncode(Ext.getCmp('bag_name').getValue()),
                                    bag_desc: Ext.htmlEncode(Ext.getCmp('bag_desc').getValue()),
                                    bag_banner: Ext.htmlEncode(Ext.getCmp('bag_banner').getValue()),
                                    bag_start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('bag_start_time').getValue()), 'Y-m-d H:i:s')),
                                    bag_end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('bag_end_time').getValue()), 'Y-m-d H:i:s')),
                                    bag_show_start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('bag_show_start_time').getValue()), 'Y-m-d H:i:s')),
                                    bag_show_end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('bag_show_end_time').getValue()), 'Y-m-d H:i:s')),
                                    product_number: Ext.htmlEncode(Ext.getCmp('product_number').getValue()),
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                        EdmContentStore.load();
                                        editWin.close();
                                    }
                                },
                                failure: function () {
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
        title: '區域包新增/編輯',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 450,
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
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initRow(row) {
        //Ext.getCmp('bag_id').show(true);
        Ext.getCmp('create_user').show(true);
        Ext.getCmp('update_user').show(true);
        Ext.getCmp('create_time').show(true);
        Ext.getCmp('update_time').show(true);
        Ext.getCmp('bag_banner').setRawValue(row.data.bag_banner);
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