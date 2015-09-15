//出貨查詢Model
Ext.define('GIGADE.deliver_master', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "order_num", type: "string" },//訂單編號
        { name: "Deliver_Id", type: "int" },//出貨編號
        { name: "Delivery_Name", type: "string" },//收貨人 
        { name: "Item_Id", type: "int" },//產品細項編號
        { name: "Product_Name", type: "string" },//產品名稱
        { name: "Buy_Num", type: "int" },//數量
        { name: "Detail_Id", type: "int" },
        { name: "Product_Freight_Set_Str", type: "string" },//狀態
        { name: "Detail_Status", type: "int" }//狀態
    ]
});

//出貨查詢列表Store
var DeliversListStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    model: 'GIGADE.deliver_master',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/SubPoenaDetailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//加載前
DeliversListStore.on('beforeload', function () {
    Ext.apply(DeliversListStore.proxy.extraParams, {
        ticket_id: document.getElementById("hid_ticket_id").value
    });
});

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 30,
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        html: '<div class="capion">傳票明細&nbsp;&nbsp;' + document.getElementById("hid_ticket_id").value + '</div>',
                        frame: false,
                        border: false
                    }
                ]
            }
        ]
    });

    //頁面加載時創建grid
    var VendorListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VendorListGrid',
        store: DeliversListStore,
        height: 740,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: '訂單編號', dataIndex: 'order_num', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "";
                    } else {
                        return Ext.String.format("<a  href=javascript:TranToOrderDetail('/OrderManage/OrderDetialList','{0}') style='text-decoration:none'>{1}</a>", record.data.order_num, record.data.order_num);
                    }
                }
            },
            {
                header: '出貨編號', dataIndex: 'Deliver_Id', width: 110, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format("<a  href=javascript:TranToDeliverDetail('/SendProduct/DeliverView','{0}') style='text-decoration:none'>{1}</a>", record.data.Deliver_Id, record.data.Deliver_Id);
                }
            },
            { header: '收貨人', dataIndex: 'Delivery_Name', width: 110, align: 'center' },
            { header: '產品細項編號', dataIndex: 'Item_Id', width: 110, align: 'center' },
            { header: '產品名稱', dataIndex: 'Product_Name', width: 110, align: 'center' },
            { header: '數量', dataIndex: 'Buy_Num', width: 110, align: 'center' },
            { header: '狀態', dataIndex: 'Product_Freight_Set_Str', width: 110, align: 'center' },
            //{ header: 'detail_id', dataIndex: 'Detail_Id', width: 110, align: 'center' },
            //{ header: 'deliver_id', dataIndex: 'Deliver_Id', width: 110, align: 'center' },
            {
                header: '功能', dataIndex: 'Detail_Status', width: 110, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.Detail_Status == 6 && record.data.Buy_Num > 1) {
                        return Ext.String.format("<a href='javascript:void(0);' onclick='NoDelivery(" + record.data.Deliver_Id + "," + record.data.Detail_Id + ")'>未到貨</a>&nbsp;&nbsp;&nbsp;<a href='javascript:void(0);' onclick='split_detail(" + record.data.Deliver_Id + "," + record.data.Detail_Id + ")'>拆分細項</a>");
                    }
                    if (record.data.Detail_Status == 6) {
                        return "<a href='javascript:void(0);' onclick='NoDelivery(" + record.data.Deliver_Id + "," + record.data.Detail_Id + ")'>未到貨</a>";
                    }
                }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                   scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        } ,
        bbar: Ext.create('Ext.PagingToolbar', {
           //store: DeliversListStore,
            //displayInfo: true,
            //displayMsg:  TOTAL + ': {2}'
          // emptyMsg: NOTHING_DISPLAY
        })
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, VendorListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VendorListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

  //  ToolAuthority();
    DeliversListStore.load();
    //VendorConditionStore.load({
    //    callback: function () {
    //        VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '全部' });
    //        Ext.getCmp("vendorcondition").setValue(VendorConditionStore.data.items[0].data.vendor_id);
    //    }
    //});
})

//跳轉到出貨明細頁
function TranToDeliverDetail(url, deliver_id) {
    var url = url + '?deliver_id=' + deliver_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#pnldeliverdetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'pnldeliverdetial',
        title: '出貨明細',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

//跳轉到訂單內容頁
function TranToOrderDetail(url, order_id) {
    var url = url + '?Order_Id=' + order_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

//未到貨？
function NoDelivery(Deliver_Id, Detail_Id) {
    Ext.MessageBox.confirm("提示", "未到貨？", function (btn) {
        if (btn == "yes") {
            Ext.Ajax.request({
                url: "/SendProduct/NoDelivery",
                method: 'post',
                params: {
                    Deliver_id: Deliver_Id,
                    Detail_id: Detail_Id
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success == "false") {
                        Ext.Msg.alert("提示", "系統錯誤");
                    } 
                    DeliversListStore.load();
                },
                error: function (sucess) {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    DeliversListStore.load();
                }
            });
            Ext.getCmp('editWin').destroy();
        }
        else {
            return false;
        }
    });
};
//拆分細項?
function split_detail(Deliver_Id, Detail_Id) {
    Ext.MessageBox.confirm("提示", "拆分細項？", function (btn) {
        if (btn == "yes") {
            Ext.Ajax.request({
                url: "/SendProduct/split_detail",
                method: 'post',
                params: {
                    "Deliver_Id": Deliver_Id,
                    "Detail_Id": Detail_Id
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success == "false") {
                        Ext.Msg.alert("提示", "正在編寫");
                        //Ext.Msg.alert("提示", "系統錯誤");
                    }
                    DeliversListStore.load();
                },
                error: function (sucess) {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    DeliversListStore.load();
                }
            });
        } else {
            return false;
        }
    })
}