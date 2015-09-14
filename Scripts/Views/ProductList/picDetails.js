var Pic;
var Pic2; //add by wwei0216w 2015/4/2 添加手機說明圖的Img
var expalinPic;
var standard;
var other

//產品規格圖model
Ext.define("picture", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "img", type: "string" },
        { name: "spec_id", type: "string" },
        { name: "spec_sort", type: "int" },
        { name: "spec_status", type: "string"}]
});



//商品說明圖model
Ext.define("explain", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "img", type: "string" },
        { name: "image_sort", type: "int" },
        { name: 'image_state', type: "string" },
        { name: 'pic_type', type: "int" },//圖片的類型 1:商品說明圖檔 2:手機說明圖檔
        {   
        name: 'picType', type: 'string', convert: function (v, record) {//圖片的類型的中文 1:商品說明圖檔 2:手機說明圖檔
        if (record.data.pic_type == 1) {
            return PRODUCT_EXPLAIN_PICS;
        }
        else {
            return PHONE_EXPLAIN_PICS;
        }
    }
}
    ]
});
//商品說明圖Store
var explainStore = Ext.create('Ext.data.Store', {
    model: 'explain',
    groupField: 'picType',
    proxy: {
        type: 'ajax',
        url: '/Product/QueryExplainPic',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    sorters: [{//edit by xiangwang0413w 2014/10/17 根據序號降序排列
        property: 'image_sort',
        direction: 'DESC'
    }]
});

//手機說明圖
//var mobileStore = Ext.create('Ext.data.Store', {
//    model: 'explain',
//    proxy: {
//        type: 'ajax',
//        url: '/Product/QueryExplainPic',
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    },
//    sorters: [{//edit by xiangwang0413w 2014/10/17 根據序號降序排列
//        property: 'image_sort',
//        direction: 'DESC'
//    }]
//});

//產品規格圖store
var StandardStore = Ext.create('Ext.data.Store', {
    model: 'picture',
    proxy: {
        type: 'ajax',
        url: '/Product/QuerySpecPic',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    sorters: [{//edit by xiangwang0413w 2014/10/17 根據序號降序排列
        property: 'spec_sort',
    direction: 'DESC'
}]
});
//規格一store
var SpecStore = Ext.create("Ext.data.Store", {
    fields: ['spec_name', 'spec_id'],
    proxy: {
        type: 'ajax',
        url: "/Product/spec1TempQuery",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

function imgFadeBig(img, size) {
    var e = this.event;
    var topValue;
    $("#imgTip").attr("src", img)
            .css({
                "top": (e.clientY - size) < 0 ? e.clientY : (e.clientY - size) + "px",
                "left": e.clientX + "px",
                "width": size + "px",
                "height": size + "px"
            }).show();
}

//組合商品圖檔介面
function PicPanel(product_id) {   
    //商品圖
    Pic = Ext.create('Ext.Img', {
        width: 150,
        title: PRODUCT_PIC,
        colName: 'product_image',
        hidden:true,
        height: 150,
        style: {
            margin: '10 0 0 0',
            border: 'solid 1px #EEE',
            padding: '3 3 3 3'
        }
    });

    Pic2 = Ext.create('Ext.Img', {
        width: 150,
        title: PHONE_EXPLAIN_PIC,
        margin: '10 0 0 20',
        height: 150,
        style: {
            border: 'solid 1px #EEE',
            padding: '3 3 3 3'
        }
    });//add by wwei0216w 2015/4/2 添加手機說明圖的Img

    other = Ext.create("Ext.form.Panel",{
        width: 900,
        border: false,
        colName: 'product_media',
        hidden: true,
        layout: 'hbox',
        items: [{
            xtype: 'displayfield',
            id: 'product_media',
            fieldLabel: PRODUCT_MEDIA,
            labelWidth: 70,
            border: false,
            readOnly:true,
            width: 480
        }]
    });

    ////手機說明圖  
    //mobilePic = Ext.create("Ext.grid.Panel", {
    //    y: 5,
    //    id: 'mobilePanel',
    //    store: mobileStore,
    //    title: "手機說明圖",
    //    width: 700,
    //    height: 200,
    //    border: true,
    //    columns: [{
    //        header: PRODUCT_EXPLAIN_PIC,
    //        sortable: false,
    //        menuDisabled: true,
    //        dataIndex: 'image_filename',
    //        //hidden: true,
    //        //colName: 'image_filename',
    //        xtype: 'templatecolumn',
    //        tpl: '<img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{img}" />',
    //        width: 200,
    //        align: "center"
    //    }, {
    //        header: PIC_SORT,
    //        sortable: false,
    //        menuDisabled: true,
    //        width: 190,
    //        dataIndex: 'image_sort',
    //        //hidden: true,
    //        //colName: 'image_sort',
    //        align: 'center'
    //    }, {
    //        header: PIC_SHOW,
    //        sortable: false,
    //        menuDisabled: true,
    //        width: 200,
    //        dataIndex: 'image_state',
    //        //hidden: true,
    //        //colName: 'image_state',
    //        align: 'center',
    //        renderer: function (value) {
    //            if (value == "1" || value == 'true') {
    //                return PIC_SHOW;
    //            }
    //            else {
    //                return PIC_NOT_SHOW;
    //            }
    //        }
    //    }]
    //});

    var grouping = Ext.create('Ext.grid.feature.Grouping', {
        groupHeaderTpl: '{name}'
    });

    //商品說明圖  
    expalinPic = Ext.create("Ext.grid.Panel", {
        features: [grouping],
        y: 5,
        id: 'explainPanel',
        store: explainStore,
        title: PRODUCT_EXPLAIN_PIC,
        width: 700,
        height: 230,
        border: true,
        columns: [{
            header: PRODUCT_EXPLAIN_PIC,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'image_filename',
            hidden: true,
            colName: 'image_filename',
            xtype: 'templatecolumn',
            tpl: '<img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{img}" />',
            width: 200,
            align: "center"
        }, {
            header: PIC_SORT,
            sortable: false,
            menuDisabled: true,
            width: 190,
            dataIndex: 'image_sort',
            hidden: true,
            colName: 'image_sort',
            align: 'center'
        }, {
            header: PIC_SHOW,
            sortable: false,
            menuDisabled: true,
            width: 200,
            dataIndex: 'image_state',
            hidden: true,
            colName: 'image_state',
            align: 'center',
            renderer: function (value) {
                if (value == "1" || value == 'true') {
                    return PIC_SHOW;
                }
                else {
                    return PIC_NOT_SHOW;
                }
            }
        }]
    });
    //規格圖
    standard = Ext.create("Ext.grid.Panel", {
        id: 'standardPanel',
        store: StandardStore,
        height: 180,
        title: SPEC_IMG,
        width: 700,
        border: true,
        columns: [{
            header: PRODUCT_SPEC_PIC,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'spec_image',
            hidden: true,
            colName: 'spec_image',
            xtype: 'templatecolumn',
            tpl: '<img width=50 height=50 onmousemove="javascript:imgFadeBig(this.src,200);" onmouseout = "javascript:$(\'#imgTip\').hide()" src="{img}" />',
            width: 200,
            align: "center"
        }, {
            header: SPEC_ONE,
            sortable: false,
            menuDisabled: true,
            width: 130,
            dataIndex: 'spec_id',
            hidden: true,
            name: 'spec_name',
            colName: 'spec_name',
            align: 'center',
            renderer: function (value) {
                var index = SpecStore.find("spec_id", value);
                var recode = SpecStore.getAt(index);
                if (recode) {
                    return recode.get("spec_name");
                }

            }
        }, {
            header: PIC_SORT,
            sortable: false,
            menuDisabled: true,
            width: 130,
            dataIndex: 'spec_sort',
            hidden: true,
            colName: 'spec1_sort',
            align: 'center'
        }, {
            header: PIC_SHOW,
            sortable: false,
            menuDisabled: true,
            width: 130,
            dataIndex: 'spec_status',
            hidden: true,
            colName: 'spec1_status',
            align: 'center',
            renderer: function (value) {
                if (value == "1" || value == 'true') {
                    return PIC_SHOW;
                }
                else {
                    return PIC_NOT_SHOW;
                }
            }
        }]
    });

    
};



function PicLoadTogether(product_id) {
    StandardStore.on('beforeload', function () {
        Ext.apply(StandardStore.proxy.extraParams,
        {
            product_id: product_id
        });
    });

    explainStore.on('beforeload', function () {
        Ext.apply(explainStore.proxy.extraParams,
        {
            product_id: product_id
        });
    });


    SpecStore.on('beforeload', function () {
        Ext.apply(SpecStore.proxy.extraParams,
        {
            ProductId: product_id
        });
    });
    

    SpecStore.load();
    StandardStore.load();
    explainStore.load({ params: { 'apporexplain': 1 } });
    //mobileStore.load({ params: { 'apporexplain': 2, product_id: product_id } });

    Ext.Ajax.request({
        type: 'ajax',
        url: '/Product/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": product_id
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            Pic.setSrc(resText.data.Product_Image);
            Pic2.setSrc(resText.data.Mobile_Image);//add by wwei0216w 2015/4/2 添加手機說明圖的Img
            if (resText.data.product_media == "") {
                Ext.getCmp("product_media").update(PRODUCT_MEDIA_NONE);
            }
            else {
                Ext.getCmp("product_media").update(PRODUCT_MEDIA+"：   <a href='" + resText.data.product_media + "' target='_Blank'>"+resText.data.product_media+"</a>");
            }
        }
    });
}