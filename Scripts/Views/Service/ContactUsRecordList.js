var CallidForm;
var pageSize = 25;
var querId;
//商品主料位管理Model
Ext.define('gigade.ContactRecord', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "response_id", type: "string" },
        { name: "user_username", type: "string" },
        { name: "response_type", type: "int" },
        { name: "response_createdate", type: "string" },
        { name: "response_content", type: "string" },
        { name: "question_content", type: "string" }

    ]
});

var RecordStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ContactRecord',
    proxy: {
        type: 'ajax',
        url: '/Service/GetRecordList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
RecordStore.on('beforeload', function () {
    Ext.apply(RecordStore.proxy.extraParams, {
        qid: querId,
        startDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateStart').getValue())), 'Y-m-d'),
        endDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateEnd').getValue())), 'Y-m-d'),
        reply_user: Ext.getCmp('reply_user').getValue(),
        limit: pageSize
    });
});
//查詢
query = function () {
    RecordStore.removeAll();
    Ext.getCmp("gbContactRecord").store.loadPage(1, {
        params: {
            qid: querId,
            startDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateStart').getValue())), 'Y-m-d'),
            endDate: Ext.Date.format(new Date(Ext.htmlEncode(Ext.getCmp('dateEnd').getValue())), 'Y-m-d'),
            reply_user: Ext.getCmp('reply_user').getValue(),
            limit: pageSize
        }
    });
    //var startDate = Ext.getCmp('dateStart').getValue();
    //var endDate = Ext.getCmp('dateEnd').getValue();
    //if (endDate) {
    //    if (startDate > endDate) {
    //        Ext.getCmp('dateStart').markInvalid(START_LARGER_THAN_END);
    //        return;
    //    }
    //}
    //Ext.getCmp('dateStart').clearInvalid();
    //Ext.getCmp('gbContactRecord').store.loadPage(1);
}
Ext.onReady(function () {
    querId = document.getElementById('hidden_group_id').value;
    var gbContactRecord = Ext.create('Ext.grid.Panel', {
        id: 'gbContactRecord',
        store: RecordStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'response_id', hidden: true, width: 90, align: 'center' },
            { header: "詢問內容", dataIndex: 'question_content', width: 250, align: 'center' },
            { header: "回覆人員", dataIndex: 'user_username', width: 120, align: 'center' },
            { header: "回覆日期", dataIndex: 'response_createdate', width: 120, align: 'center' },
            {
                header: "回覆方式", dataIndex: 'response_type', width: 120, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value)
                    {
                        case 1:
                            return "E-mail";
                            break;
                        case 2:
                            return "簡訊";
                            break;
                        case 3:
                            return "電話";
                            break;
                        case 4:
                            return "無";
                            break;
                        default:
                            break;
                    }
                }
            },
            { header: "回覆內容", dataIndex: 'response_content', width: 250, align: 'center' },
             {
                 header: "查看", xtype: 'actioncolumn', width: 100, align: 'center',
                 items: [{
                     icon: '/Content/img/icons/application_view_list.png',
                     iconCls: 'icon-cursor',
                     tooltip: "詳細信息",
                     handler: function (grid, rowIndex, colIndex) {
                         var rec = grid.getStore().getAt(rowIndex);
                         showDetail(rec, this);
                     }
                 }]
             }
        ],
        tbar: [
             {
                 xtype: 'textfield',
                 fieldLabel: "回覆人員",
                 labelWidth: 65,
                 id: 'reply_user',
                 name: 'reply_user',
                 listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             query();
                         }
                     }
                 }
             },
            {
                xtype: 'datefield',
                editable: false,
                fieldLabel: "回覆時間",
                labelWidth: 65,
                format: 'Y-m-d',
                id: 'dateStart',
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("dateStart");
                        var end = Ext.getCmp("dateEnd");
                        var s_date = new Date(start.getValue());
                        if (end.getValue() == null) {
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                        else if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    }                   
                }
            }, {
                xtype: 'displayfield',
                value: '~'
            }, {
                xtype: 'datefield',
                format: 'Y-m-d',
                editable: false,
                id: 'dateEnd',
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("dateStart");
                        var end = Ext.getCmp("dateEnd");
                        var s_date = new Date(end.getValue());
                        if (start.getValue() == null) {
                            start.setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                        }
                        else if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            start.setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                        }
                    }
                }
            }, {
                xtype: 'button',
                text: "查詢",
                iconCls: 'ui-icon ui-icon-search-2',
                handler: query
            },
           {
               xtype: 'button',
               text: "重置",
               id: 'btn_reset',
               iconCls: 'ui-icon ui-icon-reset',
               listeners: {
                   click: function () {
                       Ext.getCmp('reply_user').setValue(""),
                       Ext.getCmp('dateStart').setValue(""),
                       Ext.getCmp('dateEnd').setValue("")
                   }
               }
           }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: RecordStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gbContactRecord],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gbContactRecord.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    RecordStore.load({ params: { start: 0, limit: 25 } });
});





//顯示詳細信息
showDetail = function (row, obj) {
    var detailPanel = Ext.create('Ext.form.Panel', {
        id: 'detailPanel',
        plain: true,
        autoScroll: true,
        defaults: { anchor: "95%" },
        bodyPadding: '5',
        labelAlign: 'top',
        items: [{
            xtype: 'displayfield',
            id: 'response_id',
            fieldLabel: "流水號",
            labelStyle: 'font-weight:bold',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        },
          {
              xtype: 'displayfield',
              fieldLabel: "詢問內容",
              labelStyle: 'font-weight:bold',
              id: 'question_content',
              name: 'question_content',
              style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
          },
        {
            xtype: 'displayfield',
            fieldLabel: "回覆人員",
            labelStyle: 'font-weight:bold',
            id: 'user_username',
            name: 'user_username',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        }, {
            xtype: 'displayfield',
            fieldLabel: "回覆時間",
            labelStyle: 'font-weight:bold',
            id: 'response_createdate',
            name: 'response_createdate',
            style: { marginBottom: '10px', borderBottom: '1px solid #ced9e7', paddingBottom: '10px' }
        },
         {
             xtype: 'checkboxgroup',
             fieldLabel: '回覆方式',
             columns: 3,
             id: 'reply',
             vertical: true,
             width: 600,
             readOnly: true,
             labelStyle: 'font-weight:bold',
             items: [
             { boxLabel: 'E-mail', disabled: true, id: 'reply_mail', inputValue: '1',margin:'0 0 0 10' },
             { boxLabel: '簡訊', disabled: true, id: 'reply_sms', inputValue: '2',margin:'0 0 0 10' },
             {
                 boxLabel: '電話', disabled: true, id: 'reply_phone', inputValue: '3',margin:'0 0 0 10'
             }
             ]
         },
        {
            xtype: 'textarea',
            fieldLabel: "回覆內容",
            readOnly: true,
            labelStyle: 'font-weight:bold',
            id: 'response_content',
            name: 'response_content',
            height: '250px'
        }
        ]
    });

    docWidth = document.documentElement.clientWidth;
    docHeight = document.documentElement.clientHeight;

    var detailWin = Ext.create('Ext.window.Window', {
        width: docWidth / 3.1,
        minWidth: 500,
        height: docHeight - docHeight / 3,
        modal: true,
        resizable: false,
        constrain: true,
        iconCls: 'icon-view',
        closeAction: 'destroy',
        padding: 5,
        items: [detailPanel],
        layout: 'fit',
        listeners: {
            show: function () {
                var value = row.data.response_createdate;
                switch (row.data.response_type)
                {
                    case 1:
                        Ext.getCmp("reply_mail").setValue(true);
                        break;
                    case 2:
                        Ext.getCmp("reply_sms").setValue(true);
                        break;
                    case 3:
                        Ext.getCmp("reply_phone").setValue(true);
                        break;
                    case 0:
                        Ext.getCmp("reply_mail").setValue(false);
                        Ext.getCmp("reply_sms").setValue(false);
                        Ext.getCmp("reply_phone").setValue(false);
                        break;
                    default:
                        break;
                }
                value = value.substring(value.lastIndexOf('(') + 1, value.lastIndexOf(')'));
                value = Ext.Date.format(new Date(eval(value)), 'Y-m-d H:i');
                Ext.getCmp('response_createdate').setValue(value);
                detailPanel.loadRecord(row);
            }
        }
    });

    detailWin.show(obj);
}



