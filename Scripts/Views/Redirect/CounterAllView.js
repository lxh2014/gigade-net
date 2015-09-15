var dayNum = 31;
var group_id;
var group_name;
//獲取從當月往前推12個月
Ext.define("gigade.Countdate", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Value", type: "string" }
    ]
});
var CountdateStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Countdate',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Redirect/GetCountdate",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Ext.define('gigade.monthModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "datetime", type: "string" },
        { name: "click_total", type: "string" },
        { name: "holiday", type: "string" }
    ]
});
var dailyStore = Ext.create('Ext.data.Store', {
    model: 'gigade.monthModel',
    proxy: {
        type: 'ajax',
        url: '/Redirect/GetDailyGrid',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var WeekStore = Ext.create('Ext.data.Store', {
    model: 'gigade.monthModel',
    proxy: {
        type: 'ajax',
        url: '/Redirect/GetDailyGrid',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var HourStore = Ext.create('Ext.data.Store', {
    model: 'gigade.monthModel',
    proxy: {
        type: 'ajax',
        url: '/Redirect/GetDailyGrid',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

dailyStore.on('beforeload', function () {
    Ext.apply(dailyStore.proxy.extraParams,
       {
           selectDate: Ext.getCmp('forward_month').getValue(),
           group_id: group_id
       });
});

function getDayNum() {
    var selectdate = Ext.getCmp('forward_month').getValue();
    var d;
    d = new Date(selectdate);
    d = new Date(d.setDate(0));
    dayNum = d.getDate();
    return d.getDate();
}
Ext.onReady(function () {
    group_id = document.getElementById("group_id").value;
    group_name = document.getElementById("group_name").value;
    var titleForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        id: 'titleForm',
        border: false,
        width: 800,
        plain: true,
        bodyPadding: 20,
        //defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            html: '<div class="contain-title" >' + group_name + '群組報表</div>',
            frame: false,
            margin: '0 0 3 0',
            border: false
        },
        {
            xtype: 'fieldcontainer',
            items: [
                {
                    xtype: 'combobox',
                    fieldLabel: "選擇報表日期",
                    queryMode: 'local',
                    id: 'forward_month',
                    name: 'forward_month',
                    store: CountdateStore,
                    width: 300,
                    background: "none repeat scroll 0 0 #669841",
                    displayField: 'Value',
                    valueField: 'Value',
                    listeners: {
                        change: function (field, value) {
                            getDayNum();
                    }
                    }
                },
                {//建立時間
                    xtype: 'displayfield',
                    fieldLabel: "報表輸出時間",
                    value: new Date().toLocaleString()
                }
            ]
        }
        ]
    });
        dailyStore.load();
    //日期列表
    var dailyGrid = Ext.create('Ext.grid.Panel', {
        id: 'dailyGrid',
        store: dailyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "日期", dataIndex: 'datetime', width: 80, align: 'center', align: 'center' },
            { header: "點擊次數", dataIndex: 'click_total', width: 100, align: 'center' },
             { header: "週末", dataIndex: 'holiday', width: 100, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    //週期列表
    var weekGrid = Ext.create('Ext.grid.Panel', {
        id: 'weekGrid',
        store: dailyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: '星期', dataIndex: 'datetime', width: 80, align: 'center', align: 'center' },
            { header: '點閱次數', dataIndex: 'click_total', width: 100, align: 'center' }
        ]
    });
    //小時列表
    var hourGrid = Ext.create('Ext.grid.Panel', {
        id: 'hourGrid',
        store: dailyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: '小時', dataIndex: 'datetime', width: 80, align: 'center', align: 'center' },
            { header: '點閱次數', dataIndex: 'click_total', width: 100, align: 'center' }
        ]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [titleForm, dailyGrid, weekGrid, hourGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                // gdGroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
})
