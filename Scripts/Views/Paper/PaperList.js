var CallidForm;
var pageSize = 25;
//問卷Model
Ext.define('gigade.Paper', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "paperID", type: "string" },
        { name: "paperName", type: "string" },
        { name: "paperMemo", type: "string" },
        { name: "paperBanner", type: "string" },
        { name: "bannerUrl", type: "string" },
        //{ name: "isPromotion", type: "string" },
        //{ name: "promotionUrl", type: "string" },
        //{ name: "isGiveBonus", type: "string" },
        //{ name: "bonusNum", type: "string" },
        //{ name: "isGiveProduct", type: "string" },
        //{ name: "productID", type: "string" },
        { name: "isRepeatGift", type: "string" },
        { name: "event_ID", type: "string" },
        { name: "isRepeatWrite", type: "string" },
        { name: "isNewMember", type: "string" },
        { name: "paperStart", type: "string" },
        { name: "paperEnd", type: "string" },
        { name: "status", type: "string" },
        { name: "creator", type: "string" },
        { name: "created", type: "string" },
        { name: "modifier", type: "string" },
        { name: "modified", type: "string" },
        { name: "ipfrom", type: "string" }
    ]
});

var PaperStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Paper',
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperList',
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
            Ext.getCmp("gdPaper").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
PaperStore.on('beforeload', function () {
    Ext.apply(PaperStore.proxy.extraParams, {
        paper_name: Ext.getCmp('paper_name') == null ? "" : Ext.getCmp('paper_name').getValue()
    });
});
Ext.onReady(function () {
    var gdPaper = Ext.create('Ext.grid.Panel', {
        id: 'gdPaper',
        store: PaperStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'paperID', width: 50, align: 'center' },
            {
                header: "問卷名稱", dataIndex: 'paperName', width: 150, align: 'center'
            },
            {
                header: "問卷備註", dataIndex: 'paperMemo', width: 100, align: 'center'
            },
            //{ header: "問卷廣告", dataIndex: 'paperBanner', width: 80, align: 'center' },
            {
                header: "問卷廣告", id: 'imgsmall', colName: 'paperBanner',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value!='') {
                        return '<div style="width:50px;height:50px"><a target="_blank", href="' + record.data.paperBanner + '"><img width="50px" height="50px" src="' + record.data.paperBanner + '" /></a><div>'
                    } else {
                        return null;
                    }
                },
                width: 60, align: 'center', sortable: false, menuDisabled: true
            },
            {
                header: "問卷廣告鏈接", dataIndex: 'bannerUrl', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format('<a href="{0}" target="bank">{1}</a>', value, value);
                }
            },
            //{
            //    header: "是否鏈接促銷", dataIndex: 'isPromotion', width: 80, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value == "1") {
            //            return '是';
            //        }
            //        else {
            //            return '否';
            //        }
            //    }
            //},
            //{
            //    header: "促銷地址", dataIndex: 'promotionUrl', width: 150, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        return Ext.String.format('<a href="{0}" target="bank">{1}</a>', value, value);
            //    }
            //},
            //{
            //    header: "是否贈送購物金", dataIndex: 'isGiveBonus', width: 100, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value == "1") {
            //            return '是';
            //        }
            //        else {
            //            return '否';
            //        }
            //    }
            //},
            //{ header: "購物金金額", dataIndex: 'bonusNum', width: 80, align: 'center' },
            //{
            //    header: "是否贈送贈品", dataIndex: 'isGiveProduct', width: 80, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value == "1") {
            //            return '是';
            //        }
            //        else {
            //            return '否';
            //        }
            //    }
            //},
            //{
            //    header: "贈品", dataIndex: 'productID', width: 80, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (record.data.isGiveProduct == "1") {
            //            return value;
            //        }
            //        else {
            //            return null;
            //        }
            //    }
            //},
            {  header: "贈送活動ID", dataIndex: 'event_ID', width: 80, align: 'center'},
            {
                header: "是否重複贈送", dataIndex: 'isRepeatGift', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return '是';
                    }
                    else {
                        return '否';
                    }
                }
            },
            {
                header: "是否重複填寫", dataIndex: 'isRepeatWrite', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return '是';
                    }
                    else {
                        return '否';
                    }
                }
            },
             {
                 header: "是否為新會員填寫", dataIndex: 'isNewMember', width: 110, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == "1") {
                         return '是';
                     }
                     else {
                         return '否';
                     }
                 }
             },
            {
                header: "開始時間", dataIndex: 'paperStart', width: 150, align: 'center'
                //, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s')
            },
            {
                header: "結束時間", dataIndex: 'paperEnd', width: 150, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    var paperEnd = Date.parse(value);
                    if (paperEnd < new Date()) {
                        //return '<p style="color:#F00;">' + Ext.Date.format(new Date(value), 'Y-m-d H:i:s') + '</p>';
                        return '<p style="color:#FF0000;">' +value + '</p>';
                    }
                    else {
                        //return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
                        //return Ext.util.Format.date(value, "Y-m-d H:i:s");
                        return value;
                    }
                }
            },
            {
                header: "狀態", dataIndex: 'status', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.paperID + ")'><img hidValue='0' id='img" + record.data.paperID + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.paperID + ")'><img hidValue='1' id='img" + record.data.paperID + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            },
              {
                  header: "匯出", dataIndex: '', width: 100, align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                  {
                    
                      return "<a href='javascript:void(0);' onclick='OnePaperAnswerExcel(" + record.data.paperID + ',"' + record.data.paperName + '"' + ")'><img src='../../../Content/img/icons/excel.gif' /></a>";
                  
                  }
              }

        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          '->',
         {
             xtype: 'textfield', fieldLabel: "問卷名稱", labelWidth: 60, id: 'paper_name', listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == Ext.EventObject.ENTER) {
                         Query();
                     }
                 }
             }
         },
         //{ xtype: 'combobox', editable: false, fieldLabel: "題目", labelWidth: 55, id: 'title', store: PaperStore, displayField: 'paperName', valueField: 'paperID', value: 0 },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PaperStore,
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
        layout: 'fit',
        items: [gdPaper],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdPaper.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //PaperStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x) {
    var papername = Ext.getCmp('paper_name').getValue().trim();
    if (papername != "") {
        PaperStore.removeAll();
        Ext.getCmp("gdPaper").store.loadPage(1, {
            params: {
                paper_name: Ext.getCmp('paper_name') == null ? "" : Ext.getCmp('paper_name').getValue()

            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, "請輸入搜索內容!");
    }
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editPaperFunction(null, PaperStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdPaper").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editPaperFunction(row[0], PaperStore);
    }
}
//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Paper/UpdateState",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                PaperStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                PaperStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            PaperStore.load();
        }
    });
}
function OnePaperAnswerExcel(paper_id, paper_name)
{
    //var paper_id = Ext.getCmp('paper').getValue();
    window.open('/Paper/OutSinglePaperExcel?paper_id=' + paper_id + '&paper_name=' + paper_name);
}






