var CallidForm;
var pageSize = 25;
//商品主料位管理Model
Ext.define('gigade.BannerSite', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "banner_site_id", type: "string" },
        { name: "banner_site_sort", type: "string" },
        { name: "banner_site_status", type: "string" },
        { name: "banner_site_name", type: "string" },
        { name: "banner_site_description", type: "string" },
        { name: "banner_site_createdate", type: "string" },
        { name: "banner_site_updatedate", type: "string" },
        { name: "banner_site_ipfrom", type: "string" }
    ]
});
var BannerSiteStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.BannerSite',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerImageSiteList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

Ext.onReady(function () {
    var gdBannerSite = Ext.create('Ext.grid.Panel', {
        id: 'gdBannerSite',
        title: '圖片廣告位置',
        store: BannerSiteStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "名稱", dataIndex: 'banner_site_name', flex: 3, align: 'center' },
            {
                header: "內容", dataIndex: 'banner_site_id', flex: 1, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return "<a href='javascript:void(0);' onclick='TranToBannerImg(" + '"/Website/BannerImageList",' + value + ',0' + ")'>內容</a>";
                    //return '<a href=javascript:TranToBannerImg("/Website/BannerImageList","' + value + '","' + 0 + '")>內容</a> ';
                }
            },
            {
                header: "歷史區", dataIndex: 'banner_site_id', flex: 1, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return "<a href='javascript:void(0);' onclick='TranToBannerImg(" + '"/Website/BannerImageList",' + value + ',1' + ")'>歷史區</a>";
                    //return '<a href=javascript:TranToBannerImg("/Website/BannerImageList","' + value + '","' + 1 + '")>歷史區</a> ';
                }
            },
            { header: "描述", dataIndex: 'banner_site_description', flex: 5, align: 'center' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BannerSiteStore,
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
        items: [gdBannerSite],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdBannerSite.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    BannerSiteStore.load({ params: { start: 0, limit: 25 } });
});
function TranToBannerImg(url, id, history)
{
    var panel =window.parent.Ext.getCmp('ContentPanel');
    var record = "圖片內容列表";
    var copy = panel.down('#imag');
    if (history == 1)
    {
        record = "歷史圖片內容列表";
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
        closable: true,
        autoScroll: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}





