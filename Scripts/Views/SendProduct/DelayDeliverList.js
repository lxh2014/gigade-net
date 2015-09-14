var CallidForm;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.DeliverMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "date_pay", type: "string" },//付款日期
        { name: "count", type: "int" },//件數
    ]
});

var DelayDeliverStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.DeliverMaster',
    proxy: {
        type: 'ajax',
        url: '/SendProduct/GetDelayDeliverList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.onReady(function () {
    var DeliverMaster = Ext.create('Ext.grid.Panel', {
        id: 'DeliverMaster',
        store: DelayDeliverStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "付款日期", dataIndex: 'date_pay', width: 180, align: 'center' },
            //{
            //    header: "訂單編號", dataIndex: 'order_id', width: 120, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        return '<a href=javascript:TransToOrderDetial(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
            //    }
            //},

            { header: "件數", dataIndex: 'count', width: 160, align: 'center' },

           
            {
                header: '功能', dataIndex: 'date_pay', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TranToDetial("/SendProduct/All_In_1","' + value + '")>檢視</a>';
                }
            }
        ],
     
        bbar: Ext.create('Ext.PagingToolbar', {
            store: DelayDeliverStore,
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
        items: [DeliverMaster],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                DeliverMaster.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    DelayDeliverStore.load({ params: { start: 0, limit: 25 } });
});


//跳轉到出貨明細頁
function TranToDetial(url, deliver_id) {
    
    var start;
    var end;
    var time;
    var s = "";                           // 创建 Date 对象。
    s += deliver_id.substr(0, 4) + "/";                     // 获取年份。
    s += deliver_id.substr(5,2) + "/";              // 获取月份。
    s += deliver_id.substr(8, 2);
    //time = new Date(s);
    //end = start;
    //start = new Date(time.setMinutes(00));
    //start = new Date(time.setSeconds(00));
    //start = new Date(time.setHours(00));
    //end = new Date(time.setMinutes(59));
    //end = new Date(time.setSeconds(59));
    //end = new Date(time.setHours(23));
    //alert(start);
    //alert(end);
 
    
    var urlTran = url + '?time=' + s + '&delay='+1;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#DelayDeliver');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'DelayDeliver',
        title: "每日出貨總表",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
function TransToOrderDetial(orderId) {

    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#Deliverabel');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'Deliverabel',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}

