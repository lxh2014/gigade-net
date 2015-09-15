var Order_id;
Ext.onReady(function () {
    Order_id = GetOrderId;
    createForm();
});

function createForm() {
    //tab
    var center = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        region: 'center',
        margins: '0 5 0 0',
        activeTab: 0,
        items: [
        {
            title: '狀態列表',
            html: rtnFrame('../OrderManage/Status')
        },
        {
            title: '出貨單',
            html: rtnFrame('../OrderManage/Deliver')
        },
        {
            title: '新出貨單',
            html: rtnFrame('../OrderManage/NewDeliver')
        },
        {
            title: '物流出貨狀態',
            html: rtnFrame('../OrderManage/Logistics')
        },
        {
            title: '取消單',
            html: rtnFrame('../OrderManage/Cancel')
        },
        {
            title: '退貨單',
            html: rtnFrame('../OrderManage/OrderReturnMasterView')
        },
        {
            title: '退款單',
            html: rtnFrame('../OrderManage/Money')
        },
        {
            title: '問題與回覆',
            html: rtnFrame('../OrderManage/Question')
        },
        {
            title: '取消訂單問題',
            html: rtnFrame('../OrderManage/CancelMsg')
        },
        {
            title: '聯合信用卡',
            html: rtnFrame('../OrderManage/NCCC')
        },
        {
            title: '支付寶',
            html: rtnFrame('../OrderManage/Alipay')
        },
        {
            title: '銀聯',
            html: rtnFrame('../OrderManage/UnionPay')
        },
        {
            title: 'HappyGo',
            html: rtnFrame('../OrderManage/OrderHg')

        },
        {
            title: '購物金扣除記錄',
            html: rtnFrame('../OrderManage/OrderBonus')

        },
        {
            title: '發票記錄',
            html: rtnFrame('../OrderManage/OrderInvoice')

        },
        {
            title: '中国信托',
            html: rtnFrame('../OrderManage/OrderPaymentCt')

        },
        {
            title: '華南匯款資料',
            html: rtnFrame('../OrderManage/OrderHncb')

        },
        {
            title: 'Hitrust-網際威信',
            html: rtnFrame('../OrderManage/OrderHitrust')

        }]
    });

    Ext.create('Ext.Viewport', {
        layout: 'border',
        items: [
        center
        ]
    })
}
function rtnFrame(url) {
    return "<iframe id='if_channel' frameborder=0 width=100% height=100% src='" + url + "' ></iframe>";
}

//獲取要顯示的付款單號
function GetOrderId() {
    return document.getElementById('OrderId').value;
}