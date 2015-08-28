
Ext.define('gigade.DeliverMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "物流商", type: "string" },
        { name: "統倉包裹數", type: "int" },
        { name: "冷凍倉包裹數", type: "int" }
    ]
});

var DeliverMasterStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DeliverMaster',
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetDeliverMasterList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

function Query() {
    var falg = 0;
    var time_start = Ext.getCmp('time_start').getValue();
    var time_end = Ext.getCmp('time_end').getValue();
    if (time_start == null) {
        Ext.Msg.alert("提示", "請輸入開始時間");
        return false;
    }
    if (time_end == null) {
        Ext.Msg.alert("提示", "請輸入結束時間");
        return false;
    }
    DeliverMasterStore.removeAll();
    Ext.getCmp("gdAccum").store.loadPage(1,
        {
            params: {
                time_start: time_start,
                time_end: time_end
            }
        });
}
Ext.onReady(function () {
    var gdAccum = Ext.create('Ext.grid.Panel', {
        id: 'gdAccum',
        flex: 1.8,
        store: DeliverMasterStore,
        width: document.documentElement.clientWidth,
        columnLines: true,//顯示列線條
        frame: true,//Panel是圆角框显示
        columns: [{ header: "物流商", dataIndex: '物流商', width: 150, align: 'center' },
            { header: "統倉包裹數", dataIndex: '統倉包裹數', width: 100, align: 'center' },
              { header: "冷凍倉包裹數", dataIndex: '冷凍倉包裹數', width: 100, align: 'center' }
        ],
        tbar: [
            {
                xtype: 'datefield',
                fieldLabel: "開始時間",
                labelWidth: 80,
                width: 230,
                id: 'time_start',
                name: 'time_start',
                margin: '0 0 0 0',
                format: 'Y-m-d 00:00:00',
                editable: false,
                listeners: {
                    select: function () {
                        if (Ext.getCmp("time_start").getValue() != null) {
                            Ext.getCmp("time_end").setMinValue(Ext.getCmp("time_start").getValue());
                        }
                    }
                }
            },
            {
                xtype: 'label',
                forId: 'myFieldId',
                text: '~',
                margin: '0 0 0 5'
            },
            {
                xtype: 'datefield',
                fieldLabel: "結束時間",
                labelWidth: 80,
                width: 230,
                id: 'time_end',
                name: 'time_end',
                margin: '0 0 0 6',
                format: 'Y-m-d 23:59:59',
                editable: false,
                listeners: {
                    select: function () {
                        if (Ext.getCmp("time_end").getValue() != null) {
                            Ext.getCmp("time_start").setMaxValue(Ext.getCmp("time_end").getValue());
                        }
                    }
                }
            },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               margin: '0 0 5 6',
               handler: Query
           }
        ]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [gdAccum],
        renderTo: Ext.getBody(),
        autoScroll: true,//自動顯示滾動條
        listeners: {
            resize: function () {//在组件被调整大小之后触发,首次布局初始化大小时不触发
                gdAccum.clientWidth = document.documentElement.clientWidth;
                this.doLayout();//手动强制这个容器的布局进行重新计算。大多数情况下框架自动完成刷新布局。
            }
        }
    });
});

