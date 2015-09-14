var pageSize = 25;
/***********************群組管理主頁面****************************/
//群組管理Model
Ext.define('gigade.BannerNewsSite', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "news_site_id", type: "int" },
        { name: "news_site_sort", type: "string" },
        { name: "news_site_status", type: "int" },
        { name: "news_site_mode", type: "string" },
        { name: "news_site_name", type: "string" },
        { name: "news_site_description", type: "string" },
        { name: "news_site_createdate", type: "string" },
        { name: "news_site_updatedate", type: "string" },
        { name: "news_site_ipfrom", type: "string" },
        { name: "creattime", type: "string" },
        { name: "updtime", type: "string" }
    ]
});
var BannerNewsSite = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.BannerNewsSite',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerNewsSiteList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});

BannerNewsSite.on('beforeload', function ()
{
    Ext.apply(BannerNewsSite.proxy.extraParams, {
    });
});

Ext.onReady(function ()
{
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: BannerNewsSite,
        flex: 1.8,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store)
            {
                return "x-selectable";
            }
        },
        columns: [
            { header: "編號", dataIndex: 'news_site_id', flex: 1, align: 'center', hidden: true },
            { header: "名稱", dataIndex: 'news_site_name', flex: 2, align: 'center' },
            {
                header: "內容", dataIndex: 'news_site_id', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return "<a href='javascript:void(0);' onclick='TranToUrl(" + '"/Website/BannerNewsList",' + value + ',0' + ")'>內容</a>";
                    //return '<a href=javascript:TranToUrl("/Website/BannerNewsList","' + value + '","' + 0 + '")>' + "內容" + '</a> ';
                }
            },
            {
                header: "歷史區", dataIndex: 'news_site_id', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return "<a href='javascript:void(0);' onclick='TranToUrl(" + '"/Website/BannerNewsList",' + value + ',1' + ")'>歷史區</a>";
                    //return '<a href=javascript:TranToUrl("/Website/BannerNewsList","' + value + '","' + 1 + '")>' + "歷史區" + '</a> ';
                }
            },
            { header: "描述", dataIndex: 'news_site_description', flex: 5, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BannerNewsSite,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',//fit
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    BannerNewsSite.load({ params: { start: 0, limit: pageSize } });
});

function TranToUrl(url, id, history)
{
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var record = "文字廣告內容列表";
    var copy = panel.down('#imag');
    if (history == 1)
    {
        record = "歷史文字廣告內容列表";
        copy = panel.down('#himag');
    }
    var urlTran = url + '?sid=' + id + '&history=' + history;
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: history == 1 ? 'himag' : 'imag',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

