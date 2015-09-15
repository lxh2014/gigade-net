var CallidForm;
var pageSize = 25;

Ext.define('gigade.BannerCategory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "category_id", type: "string" },
        { name: "category_father_id", type: "string" },
        { name: "category_sort", type: "string" },
        { name: "category_name", type: "string" },
        { name: "fcategory_name", type: "string" },
        { name: "content_type", type: "string" },
        { name: "content_id", type: "string" },
        { name: "banner_site_name", type: "string" },
        { name: "description", type: "string" },
        { name: "activity", type: "string" },
        { name: "created_on", type: "string" },
        { name: "updated_on", type: "string" }
    ]
});
var BannerCategoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.BannerCategory',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerCategoryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

//獲取下拉列表的數據
var CategoryFatherStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.BannerCategory',
    proxy: {
        type: 'ajax',
        url: '/Website/GetBannerCategoryList?category_father_id=0',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//加載前
BannerCategoryStore.on('beforeload', function ()
{
    Ext.apply(BannerCategoryStore.proxy.extraParams, {
        category_father_id: Ext.getCmp('father_category').getValue()
    })

});
//var v = document.getElementById("title").value;
Ext.onReady(function ()
{
    var gdBannerCategory = Ext.create('Ext.grid.Panel', {
        id: 'gdBannerCategory',
        store: BannerCategoryStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
            { header: "標題", dataIndex: 'fcategory_name', flex: 1.5, id: 'f_name', align: 'center' },
            { header: "名稱", dataIndex: 'category_name', flex: 2.5, align: 'center' },
            {
                header: "內容", dataIndex: 'content_type', flex: 1, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    if (value == "banner")
                    {
                        return "<a href='javascript:void(0);' onclick='Tran(" + '"/Website/BannerImageList",' + record.data.content_id + ',0' + ")'>圖片</a>";
                    }
                    else if (value == "news")
                    {
                        return "<a href='javascript:void(0);' onclick='Tran(" + '"/Website/BannerNewsList",' + record.data.content_id + ',1' + ")'>文字</a>";
                    }
                }
            },
            { header: "描述", dataIndex: 'description', flex: 5, align: 'center' }
        ],
        tbar: [
          {
              xtype: 'combobox',
              id: 'father_category',
              fieldLabel: '標題',
              labelWidth: 40,
              colName: 'father_category',
              queryMode: 'local',
              editable: false,
              store: CategoryFatherStore,
              displayField: 'category_name',
              valueField: 'category_id',
              value: -1,
              listeners: {
                  select: function (combo, records, eOpts)
                  {
                      BannerCategoryStore.removeAll();
                      BannerCategoryStore.load({ params: { category_father_id: Ext.getCmp('father_category').getValue(), start: 0, limit: 25 } });
                      if (Ext.getCmp("father_category").getValue().toString() != "")
                      {
                          if (parseInt(Ext.getCmp("father_category").getValue()) > 0)
                          {
                              Ext.getCmp("f_name").hide();
                          }
                          else
                          {
                              Ext.getCmp("f_name").show();
                          }

                      }
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BannerCategoryStore,
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
        layout: 'vbox',
        items: [gdBannerCategory],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdBannerCategory.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    BannerCategoryStore.load({ params: { start: 0, limit: 25 } });
    CategoryFatherStore.load({
        callback: function ()
        {
            CategoryFatherStore.insert(0, { category_id: '-1', category_name: '請選擇' });
            Ext.getCmp("father_category").setValue(-1);
        }
    });

});
   
function Tran(url, id, type)
{
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var record = "圖片廣告列表";
    var copy = panel.down('#imag');
    if (type == 1)
    {
        record = "文字廣告列表";
        copy = panel.down('#himag');
    }
    var urlTran = url + '?sid=' + id;
    if (copy)
    {
        copy.close();
    }
    copy = panel.add({
        id: type == 1 ? 'himag' : 'imag',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function ()
{
    //addWin.show();
    editBannerImageFunction(null, BannerSiteStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function ()
{
    var row = Ext.getCmp("gdBannerSite").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0)
    {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1)
    {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1)
    {
        if (document.getElementById('history').value == "0")
        {
            editBannerImageFunction(row[0], BannerSiteStore);
        }
        else
        {
            editBannerImageHistoryFunction(row[0], BannerSiteStore);
        }
    }
}
function ColorBox()
{
    if (document.getElementById('history').value == "0")
    {
        $("a[rel='img_show']").colorbox();
    }
}







