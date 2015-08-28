var CallidForm;
var pageSize = 25;
Ext.define('gigade.BannerSite', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "banner_content_id", type: "string" },
        { name: "banner_site_id", type: "string" },
        { name: "banner_title", type: "string" },
        { name: "banner_link_url", type: "string" },
        { name: "banner_link_mode", type: "string" },
        { name: "banner_sort", type: "string" },
        { name: "banner_status", type: "string" },
        { name: "banner_image", type: "string" },
        { name: "banner_start", type: "string" },
        { name: "banner_end", type: "string" },
        { name: "banner_createdate", type: "string" },
        { name: "banner_updatedate", type: "string" },
        { name: "banner_ipfrom", type: "string" }
    ]
});
var BannerSiteStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.BannerSite',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerImageList?history=' + document.getElementById("history").value + "&&sid=" + document.getElementById("sid").value,
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdBannerSite").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function () {
    var titleForm = Ext.create('Ext.form.Panel', {
        id: 'titleForm',
        border: false,
        plain: true,
        bodyPadding: 10,
        layout: 'anchor',
        //defaults: { anchor: "95%", msgTarget: "side" },
        items: [
               {
                   xtype: 'displayfield',
                   id: 'site_name',
                   name: 'site_name',
                   value: '<div style="font-size:20px;width:900px;">廣告區塊位置&nbsp;&nbsp;&nbsp;' + document.getElementById('sname').value + '</div>'
               }
        ]
    });
    var image = '';
    if (document.getElementById('history').value != "1") {
       image = '<a href="{banner_image}" rel="img_show"><img name="tplImg" height=30 width=50 border=0  src="{banner_image}"  onclick="ColorBox()" /></a>';
    }
    else {
        image = '<img name="tplImg" height=30 width=50 border=0  src="{banner_image}"  />';
    }
    var gdBannerSite = Ext.create('Ext.grid.Panel', {
        id: 'gdBannerSite',
        store: BannerSiteStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 3.8,
        columns: [
            {
                header: "圖示",
                dataIndex: 'banner_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                //tpl: '<a href="{banner_image}" rel="img_show"><img name="tplImg" height=30 width=50 border=0  src="{banner_image}"'+document.getElementById('history').value != "1"?' onclick="ColorBox()"':''+' /></a>' 
                tpl:image
            },
            { header: "流水號", dataIndex: 'banner_content_id', width: 80, align: 'center' },
            { header: "標題", dataIndex: 'banner_title', width: 300,id:'b_title', align: 'center' },
            {
                header: "開啟模式", dataIndex: 'banner_link_mode', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return '<p style="color:red;">母視窗連結</p>';
                    }
                    else if (value == "2") {
                        return '新視窗開啟';
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'banner_status', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0") {
                        return '<p style="color:#666;">新建</p>';
                    }
                    else if (value == "1") {
                        return '<p style="color:#00F;">顯示</p>';
                    }
                    else if (value == "2") {
                        return '<p style="color:red;">隱藏</p>';
                    }
                    else {
                        return '<p style="color:#F00;">下線</p>';
                    }
                }
            },
            { header: "排序", dataIndex: 'banner_sort', width: 80, align: 'center' },
            {
                header: "上線時間", dataIndex: 'banner_start', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return value.toString().substr(0,16);
                }
                //, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s')
            },
            {
                header: "下線時間", dataIndex: 'banner_end', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    var banner_end = Date.parse(value);
                    if (banner_end < new Date()) {
                        //return '<p style="color:#F00;">' + Ext.Date.format(new Date(value), 'Y-m-d H:i:s') + '</p>';
                        return '<p style="color:#F00;">' + value.toString().substr(0, 16) + '</p>';
                    }
                    else {
                        //return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
                        return value.toString().substr(0, 16);
                    }
                }
            },
            {
                header: "連結", dataIndex: 'banner_link_url', width: 450, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    var target = '';
                    if (record.data.banner_link_mode == "2") {
                        target = ' target = "_blank"';
                    }
                    else {
                        target = 'target="_self"';

                    }
                    return Ext.String.format('<a href="{0}" {1}>{2}</a>', record.data.banner_link_url, target, record.data.banner_link_url);
                }
            }
        ],
        tbar: [
          { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
          { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           {
               xtype: 'button', text: "回上頁", id: 'back', hidden: false, handler: function ()
               {
                   var tab;
                   var panel = window.parent.parent.Ext.getCmp('ContentPanel');
                   var imag = panel.down('#imag');
                   var himag = panel.down('#himag');
                   if (imag) { tab = imag; }
                   if (himag) { tab = himag;}
                   tab.close();
               }
           }
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
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [titleForm, gdBannerSite],
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
    if (document.getElementById('history').value == "1")
    {
        Ext.getCmp("add").hide();
        Ext.getCmp("b_title").hide();
        document.getElementById('edit-btnInnerEl').innerHTML = '查看';
    }
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editBannerImageFunction(null, BannerSiteStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdBannerSite").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        if (document.getElementById('history').value == "0") {
            editBannerImageFunction(row[0], BannerSiteStore);
        }
        else {
            editBannerImageHistoryFunction(row[0], BannerSiteStore);
        }
    }
}
function ColorBox() {
    if (document.getElementById('history').value == "0")
    {
        $("a[rel='img_show']").colorbox();
}
}







