var bid = 222;    //
var i = 2;
var win;
var errorMsg;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
Ext.define('gigade.Image', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "int" },
        { name: "image_filename", type: "string" },
        { name: "image_sort", type: "int" },
        { name: "image_state", type: "int" },
        { name: "image_createdate", type: "int" }
    ]
});

var ImageStore = Ext.create('Ext.data.Store', {
    autoLoad: true,
    model: 'gigade.Image',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetImageInfo',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var ThisTypeStore = new Ext.data.SimpleStore({
    fields: ['value', 'name'],
    data: [
        ['1', '顯示'],
        ['2', '隱藏']
    ]
});


ImageStore.on('beforeload', function () {
    Ext.apply(ImageStore.proxy.extraParams, {
        brand_id: document.getElementById("brandid").value
    })
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("mygrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("mygrid").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

//頁面載入
Ext.onReady(function () {
    var pictureMaintainForm = Ext.create("Ext.form.Panel", {
        id: "form",
        url: "/Vendor/UploadPicture",
        frame: true,
        plain: true,
        defaultType: "textfield",
        layout: "anchor",
        labelWidth: 45,
        items: [
            {
                xtype: 'displayfield',
                id: 'group_id',
                fieldStyle: "font-size:18px;color:orange;",
                value: "【" + document.getElementById("brandname").value + "】品牌故事圖檔維護",
                name: 'brandname'
            },
            {
                xtype: "label",
                text: "※檔案格式只支援 GIF, JPG, PNG 三種格式，其它格式將略過不處理，建議尺寸760xN，圖片大小限制150K！"
            },
            {
                xtype: "filefield",
                name: "photo",
                id: "photo",
                fieldLabel: "圖檔1",
                msgTarget: "side",
                anchor: "95%",
                //                width: 200,
                allowBlank: false,
                buttonText: '選擇..',
                submitValue: true,
                //                allowBlank: true,
                fileUpload: true
            }
        ],
        buttons: [
            {
                text: "增加欄位",
                handler: function () {
                    var _form = Ext.getCmp("form");
                    var fileField = new Ext.form.field.File({
                        //                        width: 200,
                        anchor: "95%",
                        allowBlank: false,
                        buttonText: '選擇..',
                        id: "item" + i,
                        fieldLabel: "圖檔" + i
                    });
                    _form.add(fileField);
                    _form.doLayout();
                    i++;
                }
            },
            {
                text: "移除欄位",
                handler: function () {
                    var _form = Ext.getCmp("form");
                    var dd = i - 1;
                    var fileFieldItem = Ext.getCmp("item" + dd.toString());
                    _form.remove(fileFieldItem, true);
                    _form.doLayout();
                    if (i > 2) {    //避免出現負數
                        i--;
                    }
                }
            },
            {
                text: "上傳圖片",
                handler: function () {
                    var form = this.up("form").getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                waitMsg: "Uploading your photo...",
                                brand_id: document.getElementById("brandid").value
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg != "undefined") {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                        ImageStore.load();
                                    } else {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                        ImageStore.load();
                                    }
                                } else {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText)
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        });
                    }
                }
            }
        ]
    });
    var mygrid = Ext.create('Ext.grid.Panel', {
        id: 'mygrid',
        store: ImageStore,
        viewConfig: {
            forceFit: true, getRowClass:
           function (record, rowIndex, rowParams, store) {
               if (record.data.image_state == 2) {
                   return 'changeColor';
               } else {
                   return "";
               }
           }
        },
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "圖示",
                dataIndex: 'image_filename',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{image_filename} " ><img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{image_filename}" /></a>'
            },
            {
                header: "排序", dataIndex: 'image_sort', width: 160, align: 'center',
                editor: {
                    xtype: 'numberfield',
                    id: 'numbers',
                    minValue: 0,
                    maxValue:99
                }
            },
            {
                header: "顯示/隱藏",
                dataIndex: 'image_state',
                width: 160,
                align: 'center',
                editor: {
                    xtype: 'combobox',
                    id: 'this_type',
                    name: 'this_type',
                    store: ThisTypeStore,
                    hideLabel: true,
                    lazyRender: true, //值为true时阻止ComboBox渲染直到该对象被请求
                    displayField: "name",
                    valueField: "value",
                    mode: "local",
                    editable: false,
                    triggerAction: "all"
                } ,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "顯示";
                    } else if (value == 2) {
                        return "<span style='color:red;'border='1px'>隱藏</span>";
                    }
                }
            },
            {
                header: "是否刪除", dataIndex: 'image_filename', width: 150, align: 'center',//H鎖定
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);' onclick='onRemoveClick()'><img  src='../../../Content/img/icons/delete.gif'/></a>";
                   // return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='F' id='img" + record.data.row_id + "' src='../../../Content/img/icons/hmenu-unlock.png'/></a>";
                }
            }
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        tbar: [
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', hidden:false,disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', iconCls: 'icon-user-remove', hidden:false,disabled: true, handler: onRemoveClick },
            { xtype: 'button', text: UPLOAD_MANY_ONETIME, id: 'add_manypic', iconCls: 'icon-add', hidden: false, disabled: false, handler: onUploadClick },
            //,
            //{
            //    xtype: 'button',
            //    text: '返回',
            //    iconCls: 'icon_rewind',
            //    handler: function () {
            //        window.location.href = '/Vendor/VendorBrandList';
            //    }
            //}
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
            , edit: function (editor, e) {
                if (e.field == "image_sort") {
                    if (e.value != e.originalValue) {
                        Ext.Ajax.request({
                            url: "/Vendor/UpdateSortByPicture",
                            method: 'post',
                            type: 'text',
                            params: {
                                picture: e.record.data.image_filename,
                                sort_id: e.value,
                                this_type: e.record.data.image_state
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    //Ext.Msg.alert("提示信息", "操作成功!");
                                    ImageStore.load();
                                }
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert(INFORMATION, "操作失敗!");
                            }
                        });

                    }
                }
                if (e.field == "image_state") {
                    if (e.value != e.originalValue) {
                        Ext.Ajax.request({
                            url: "/Vendor/UpdateSortByPicture",
                            method: 'post',
                            type: 'text',
                            params: {
                                picture: e.record.data.image_filename,
                                sort_id: e.record.data.image_sort,
                                this_type: e.value
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    //Ext.Msg.alert("提示信息", "操作成功!");
                                    ImageStore.load();
                                }
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert(INFORMATION, "操作失敗!");
                            }
                        });

                    }
                }
            }
        }
        ,selModel: sm
    });
    //內容展示
    Ext.create("Ext.container.Viewport", {
        layout: "anchor",
        items: [pictureMaintainForm, mygrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                mygrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});

function imgFadeBig(img, width, height) {
    var e = this.event;
    if (img.split('/').length != 5) {
        $("#imgTip").attr("src", img)
            .css({
                "top": (e.clientY < height ? e.clientY : e.clientY - height) + "px",
                "left": (e.clientX) + "px",
                "width": width + "px",
                "height": height + "px"
            }).show();
    }
}

onAddClick = function () {
    editFunction(null, ImageStore);
}

//修改
onEditClick = function () {
    var row = Ext.getCmp("mygrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ImageStore);
    }
}
//刪除
onRemoveClick = function () {
    var row = Ext.getCmp("mygrid").getSelectionModel().getSelection();
    if (row.length < 1) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.image_filename + '|';
                }
                Ext.Ajax.request({
                    url: '/Vendor/DeleteImage',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);

                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            ImageStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "操作失敗");
                        }
                    }
                });
            }
        });
    }
}
//批次上傳所有圖片
onUploadClick=function(){
    if (!win)
    {
        
        win = Ext.create('Ext.window.Window', {
            title: UPLOAD_MANY_ONETIME,
            height: 450,
            frame: false,
            border: false,
            width: 410,
            listeners: {
                close: function (e)
                {
                    win = undefined;
                }
            },
            tbar: [{
                html: '<input type="file" id="uploadify" name = "uploadify" />',
                width: 118,
                height: 35
            }, {
                xtype: 'button',
                text: BEGIN_LOAD,
                handler: function ()
                {
                    $("#uploadify").uploadifyUpload();
                }
            }, {
                xtype: 'button',
                text: STOP_LOAD,
                handler: function ()
                {
                    $('#uploadify').uploadifyClearQueue();
                }
            }],
            items: [{
                html: "<div id='fileQueue' style='width: 450px;height: 400px;overflow: auto;border: 1px solid #E5E5E5;margin-bottom: 10px;'></div>"
            }]
        });
    }
    if (win.isVisible())
    {
        win.close(this);
        win = undefined;
    }
    else
    {
        win.show(this);
    }
    brand_id = document.getElementById("brandid").value;
    errorMsg = "";
    var sort_repeat = false;
    $("#uploadify").uploadify({
        'uploader': '/Scripts/jquery.uploadify-v2.1.0/uploadify.swf',
        'script': '/Vendor/upLoadImg?brand_id=' + brand_id ,
        'cancelImg': '/Scripts/jquery.uploadify-v2.1.0/cancel.png',
        'folder': 'UploadFile',
        'queueID': 'fileQueue',
        'fileExt': '*.gif;*.jpg;*.png',
        'fileDesc': '*.gif;*.jpg;*.png',
        //'buttonImg': '/img.gigade100.com/product/nopic_150.jpg',
        'buttonText': SELECT_IMG,
        'auto': false,
        'multi': true,
        //'scriptData': { ASPSESSID: window.parent.GethfAspSessID(), AUTHID: window.parent.GethfAuth() },
      
        'onComplete': function (event, queueId, fileObj, response, data)
        {
            
            var resText = eval("(" + response + ")"); 
            if (resText.success)
            {
                if (data.fileCount >= 0)
                {
                    errorMsg += resText.msg + '<br/>';
                    if (resText.sort_repeat)
                    {
                        sort_repeat = true;
                    }
                }
                if (data.fileCount == 0)
                {
                    ImageStore.load();
                    setTimeout(function () { win.close(); }, 500);
                    
                    if (sort_repeat)
                    {
                        alert("圖片排序存在重複,請修改！");
                    }                   
                    Ext.Msg.alert("提示", errorMsg);
                }
                
            }
        }
    });
}