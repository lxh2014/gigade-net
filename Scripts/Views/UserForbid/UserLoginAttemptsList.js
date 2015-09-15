var pageSize = 25;
Ext.define('gigade.UserLogin', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "login_id", type: "string" },
        { name: "login_mail", type: "string" },
        { name: "login_ipfrom", type: "string" },
        //{ name: "user_id", type: "string" },
        //{ name: "user_name", type: "string" },
        { name: "login_createdate", type: "string" },
        { name: "sumtotal", type: "string" },
        { name: 'login_type', type: "string" },
        { name: 'slogin_type', type: "string" }
    ]
});
var UserLoginStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.UserLogin',
    proxy: {
        type: 'ajax',
        url: '/UserForbid/GetUserLoginList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前
UserLoginStore.on('beforeload', function ()
{
    Ext.apply(UserLoginStore.proxy.extraParams, {
        login_mail: Ext.getCmp('login_mail').getValue(),
        login_ipfrom: Ext.getCmp('login_ipfrom').getValue(),
        start_date: Ext.getCmp('start').getValue(),
        end: Ext.getCmp('end').getValue(),
        sumtotal: Ext.getCmp('sumtotal').getValue(),
        ismail: Ext.getCmp('ismail').getValue(),
        login_type: Ext.getCmp('login_type').getValue()
    })
});
Ext.onReady(function ()
{
    //頁面加載時創建grid
    var gdUserLogin = Ext.create('Ext.grid.Panel', {
        id: 'gdUserLogin',
        store: UserLoginStore,
        flex: 8,
        columnLines: true,
        frame: true,
        columns: [
            { xtype: 'rownumberer', header: '序號', width: 50, align: 'center' },
            //{ header: '用戶編號', dataIndex: 'user_id', width: 80, align: 'center' },
            //{ header: '用戶名稱', dataIndex: 'user_name', width: 80, align: 'center'},
            { header: '用戶郵箱', dataIndex: 'login_mail', id: 'mail', width: 200, align: 'center' },
            { header: '來源IP', dataIndex: 'login_ipfrom', id: 'ipfrom', width: 150, align: 'center' },
            //{ header: '創建時間', dataIndex: 'login_createdate', width: 150, align: 'center' },
            { header: '登入錯誤類型', dataIndex: 'slogin_type', width: 150, align: 'center' },
            { header: '登入失敗次數', dataIndex: 'sumtotal', width: 100, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserLoginStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [frm, gdUserLogin],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdUserLogin.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    UserLoginStore.load({ params: { start: 0, limit: 25 } });
})
