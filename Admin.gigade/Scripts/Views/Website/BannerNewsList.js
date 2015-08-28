var CallidForm;
var pageSize = 25;
//文字廣告Model
Ext.define('gigade.BannerNewsContent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "news_id", type: "string" },
        { name: "news_site_id", type: "string" },
        { name: "news_title", type: "string" },
        { name: "news_content", type: "string" },
        { name: "news_link_url", type: "string" },
        { name: "news_link_mode", type: "string" },
        { name: "news_sort", type: "string" },
        { name: "news_status", type: "string" },
        { name: "news_start", type: "string" },
        { name: "news_end", type: "string" },
        { name: "news_createdate", type: "string" },
        { name: "news_updatedate", type: "string" },
        { name: "news_ipfrom", type: "string" }
    ]
});
var BannerNewsContentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: true,
    pageSize: pageSize,
    model: 'gigade.BannerNewsContent',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerNewsList?history=' + document.getElementById("history").value + '&&sid=' + document.getElementById("sid").value,
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("gdBannerNewsContent").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function ()
{
    Ext.tip.QuickTipManager.init();
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        //layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'displayfield',
                    id: 'site_name',
                    name: 'site_name',
                    value: '<div style="font-size:20px;width:900px;">廣告區塊位置&nbsp;&nbsp;&nbsp;' + document.getElementById('sname').value + '</div>'
                }
            ]
        }
        ]
    });
    var gdBannerNewsContent = Ext.create('Ext.grid.Panel', {
        id: 'gdBannerNewsContent',
        store: BannerNewsContentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        flex: 9.3,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'news_id', flex: 1, align: 'center' },
            {
                header: "開啟模式", dataIndex: 'news_link_mode', flex: 1.5, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value == "1")
                    {
                        return '<p style="color:red;">母視窗連結</p>';
                    }
                    else if (value == "2")
                    {
                        return '新視窗開啟';
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'news_status', flex: 1, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value == "0")
                    {
                        return '<p style="color:#666;">新建</p>';
                    }
                    else if (value == "1")
                    {
                        return '<p style="color:#00F;">顯示</p>';
                    }
                    else if (value == "2")
                    {
                        return '<p style="color:red;">隱藏</p>';
                    }
                    else
                    {
                        return '<p style="color:#F00;">下線</p>';
                    }
                }
            },
            { header: "排序", dataIndex: 'news_sort', flex: 1, align: 'center' },
            {
                header: "上線時間<br />下線時間", dataIndex: '', flex: 1.5, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    var news_end = Date.parse(record.data.news_end);
                    if (news_end < new Date())
                    {
                        return record.data.news_start.toString().substr(0, 10) + '<br/><p style="color:#F00;">' + record.data.news_end.toString().substr(0, 10) + '</p>';
                    }
                    else
                    {
                        return record.data.news_start.toString().substr(0, 10) + '<br/>' + record.data.news_end.toString().substr(0, 10);
                    }
                }
            },
            {
                header: "文字標題<br />文字連結", dataIndex: '', flex: 4, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (record.datanews_link_mode == 2)
                    {
                        return record.data.news_title + '<br /><a href="' + record.data.news_link_url + '" target="_blank">' + record.data.news_link_url + '</a>';
                    }
                    else
                    {
                        return record.data.news_title + '<br /><a href="' + record.data.news_link_url + '">' + record.data.news_link_url + '</a>';
                    }
                }
            }
        ],
        tbar: [
              { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick }
         ,{ xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }
         ,{
        xtype: 'button', text: "回上頁", id: 'back', hidden: false, handler: function ()
        {
            var tab;
            var panel = window.parent.parent.Ext.getCmp('ContentPanel');
            var imag = panel.down('#hnews');
            var himag = panel.down('#news');
            if (imag) { tab = imag; }
            if (himag) { tab = himag;}
            tab.close();
        }
    }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BannerNewsContentStore,
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
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,gdBannerNewsContent],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdBannerNewsContent.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    BannerNewsContentStore.load({ params: { start: 0, limit: 25, history: document.getElementById("history").value, sid: document.getElementById("sid").value } });
    if (document.getElementById('history').value == "1") {
        document.getElementById('edit-btnInnerEl').innerHTML = '查看';
        Ext.getCmp('add').hide();
    }
});

/***************************新增**********************/
onAddClick = function () {
    //addWin.show();
    editBannerNewsFunction(null, BannerNewsContentStore);
}
/***************************編輯**********************/
onEditClick = function ()
{
    var row = Ext.getCmp("gdBannerNewsContent").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0)
    {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1)
    {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1)
    {
        editBannerNewsFunction(row[0], BannerNewsContentStore);
    }
}









