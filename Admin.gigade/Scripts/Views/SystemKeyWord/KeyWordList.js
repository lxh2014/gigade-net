/*
* 文件名稱 :KeyWordList.js
* 文件功能描述 :系統關鍵字列表
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
var pageSize = 30;
var maxMonth = 3;

//列表Model
Ext.define('gigade.KeyWord', {
    extend: 'Ext.data.Model',
    fields:
        [
            { name: "row_id", type: "int" },
            { name: "key_word", type: "string" },
            { name: "flag", type: "string" },
            { name: "kdate", type: "string" },
            { name: "kuser", type: "string" },
            { name: "mddate", type: "string" },
            { name: "moduser", type: "string" }
        ]
});

//到Controller獲取數據
var SystemKeyStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,//每頁最大數據,傳到前臺 
    model: 'gigade.KeyWord',
    proxy: {
        type: 'ajax',
        url: '/SystemKeyWord/GetSystemKeyWord',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gridList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gridList").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
function onAddClick() {
    AddFunction();
}

function onEditClick() {
    var row = Ext.getCmp("gridList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "未選中任何行!");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能选择一行");
    } else if (row.length == 1) {
        EditFunction(row[0], SystemKeyStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("gridList").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var ids = '';
                for (var i = 0; i < row.length; i++) {
                    ids += row[i].data.row_id + '|';
                }
                Ext.Ajax.request({
                    url: '/SystemKeyWord/DelSystemKeyWord',
                    method: 'post',
                    params: { row_id: ids },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            var totalcount = SystemKeyStore.getCount();
                            if (totalcount != 0) {
                                SystemKeyStore.load();
                            }
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗");
                            SystemKeyStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

SystemKeyStore.on('beforeload', function () {
    searchKey = Ext.getCmp("searchKey").getValue();//搜索關鍵字
    startTime = Ext.getCmp("startTime").getValue();//開始時間
    endTime = Ext.getCmp("endTime").getValue();//結束時間

        Ext.apply(SystemKeyStore.proxy.extraParams, {
            searchKey: searchKey,
            startTime: startTime,
            endTime: endTime
        });
    //}

});
SystemKeyStore.on('load', function (ListStore) {
    var totalcount = SystemKeyStore.getCount();
    if (totalcount == 0) {
        Ext.MessageBox.alert("提示信息", "  ~沒有符合條件的數據～  ");
    }
});
///搜索-Query
function Query() {
    searchKey = Ext.getCmp("searchKey").getValue();//搜索關鍵字
    startTime = Ext.getCmp("startTime").getValue();//開始時間
    endTime = Ext.getCmp("endTime").getValue();//結束時間
    if (searchKey == "" && startTime == null) {
        Ext.MessageBox.alert("提示信息", "請選擇搜索條件!");
        return;
    }
    Ext.getCmp("gridList").store.loadPage(1, {
        params: {
            searchKey: searchKey,
            startTime: startTime,
            endTime: endTime
        }
    });
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    else {
        s.setHours(23, 59, 59);
    }
    return s;
}

Ext.onReady(function () {
    gridList = Ext.create('Ext.grid.Panel', {
        id: 'gridList',
        store: SystemKeyStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.8,
        columns:
            [
            { header: "編號", dataIndex: 'row_id', flex: 1, align: 'center' },
            { header: "關鍵字", dataIndex: 'key_word', flex: 1, align: 'center' },
            {
                header: "食安關鍵字", dataIndex: 'flag', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (record.data.flag) {
                        case "0":
                            return "否";
                            break;
                        case "1":
                            return "是";
                            break;
                        default:
                            return "未知";
                    }
                }
            },
            { header: "創建人", dataIndex: 'kuser', flex: 1, align: 'center' },
            { header: "創建時間", dataIndex: 'kdate', flex: 1, align: 'center' },
            { header: "修改人", dataIndex: 'moduser', flex: 1, align: 'center' },
            { header: "修改時間", dataIndex: 'mddate', flex: 1, align: 'center' }

            ],
        tbar: [
          {
              xtype: 'button',
              text: '新增',
              id: 'add',
              iconCls: 'icon-user-add',
              handler: onAddClick
          },
          {
              xtype: 'button',
              text: '編輯',
              id: 'edit',
              iconCls: 'icon-user-edit',
              disabled: true,
              handler: onEditClick
          },
          {
              xtype: 'button',
              text: '刪除',
              id: 'remove',
              iconCls: 'icon-user-remove',
              disabled: true,
              handler: onDeleteClick
          },
         { xtype: 'button', id: 'Export', text: "匯入Excel", icon: '../../../Content/img/icons/excel.gif', hidden: false, handler: ImportExcel },
         { xtype: 'button', text: "匯出Excel", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportExcel },
          '->',
         {
             xtype: 'textfield',
             fieldLabel: '關鍵字',
             labelWidth: 60,
             width: 160,
             id: 'searchKey',
             colName: 'searchKey',
             margin: '0 5px',
             submitValue: false,
             name: 'searchKey',
             listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == e.ENTER) {
                         Query();
                     }
                 }
             }
         },
         {
             xtype: 'datetimefield',
             id: 'startTime',
             name: 'startTime',
             fieldLabel: "時間範圍",
             margin: '0 5',
             labelWidth: 60,
             width: 210,
             format: 'Y-m-d H:i:s',
             time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
             editable: false,
             //value: setNextMonth(Date.now(), -maxMonth),
             listeners: {
                 select: function (a, b, c) {
                     var startTime = Ext.getCmp("startTime");
                     var endTime = Ext.getCmp("endTime");
                     if (endTime.getValue() == null) {
                         endTime.setValue(setNextMonth(startTime.getValue(), maxMonth));
                     } else if (startTime.getValue() > endTime.getValue()) {
                         Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                         endTime.setValue(setNextMonth(startTime.getValue(), maxMonth));
                     }
                     else if (endTime.getValue() > setNextMonth(startTime.getValue(), maxMonth)) {
                         endTime.setValue(setNextMonth(startTime.getValue(), maxMonth));
                     }
                 },
                 specialkey: function (field, e) {
                     if (e.getKey() == e.ENTER) {
                         Query();
                     }
                 }
             }
         },
          {
              xtype: 'displayfield',
              value: '~',
              margin: '0 10'
          },
          {
              xtype: 'datetimefield',
              id: 'endTime',
              name: 'endTime',
              width: 150,
              margin: '0 3',
              format: 'Y-m-d H:i:s',
              editable: false,
              time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59    3
              //value: setNextMonth(Date.now(), 0),
              listeners: {
                  select: function (a, b, c) {
                      var startTime = Ext.getCmp("startTime");
                      var endTime = Ext.getCmp("endTime");
                      var s_date = new Date(startTime.getValue());
                      var now_date = new Date(endTime.getValue());
                      if (startTime.getValue() != "" && startTime.getValue() != null) {
                          if (endTime.getValue() < startTime.getValue()) {
                              Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                              endTime.setValue(setNextMonth(startTime.getValue(), maxMonth));
                          } else if (endTime.getValue() > setNextMonth(startTime.getValue(), maxMonth)) {
                              startTime.setValue(setNextMonth(endTime.getValue(), -maxMonth));
                          }

                      } else {
                          startTime.setValue(setNextMonth(endTime.getValue(), -maxMonth));
                      }
                  },
                  specialkey: function (field, e) {
                      if (e.getKey() == e.ENTER) {
                          Query();
                      }
                  }
              }
          },
             //=======查詢====重置=============
          {
              xtype: 'button',
              iconCls: 'icon-search',
              text: "查詢",
              margin: '0 10',
              handler: Query
          },
          {
              xtype: 'button',
              text: '重置',
              id: 'btn_reset',
              iconCls: 'ui-icon ui-icon-reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("searchKey").setValue('');
                      Ext.getCmp("startTime").setValue('');
                      Ext.getCmp("endTime").setValue('');
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SystemKeyStore,
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
        items: [gridList],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                gridList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

//**************************************************************    汇入  ************************************************************************
ImportExcel = function () {
    ExportFunction();
}
//**************************************************************    汇出  *************************************************************************
ExportExcel = function () {

    var time1 = "";
    var time2 = "";
    var searchKey = Ext.getCmp("searchKey").getValue();
    var startTime = Ext.getCmp("startTime").getValue();
    var endTime = Ext.getCmp("endTime").getValue();

    if (searchKey == "" && startTime == null) {
        Ext.MessageBox.alert("提示信息", "請選擇匯出條件!");
        return false
    }
    else {
        if (startTime != null && endTime != null) {
            time1 = Ext.htmlEncode(Ext.Date.format(new Date(startTime), 'Y-m-d H:i:s'));
            time2 = Ext.htmlEncode(Ext.Date.format(new Date(endTime), 'Y-m-d H:i:s'))
        }
        window.open("/SystemKeyWord/KeyWordExcelList?searchKey=" + searchKey + "&startTime=" + time1 + "&endTime=" + time2);
    }

}
