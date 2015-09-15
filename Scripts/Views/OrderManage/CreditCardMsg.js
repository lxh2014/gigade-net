
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.create('Ext.data.Store', {
    storeId: 'CreditCardMsgStore',
    fields: ["receiveid", "reason", "receivecode", "remark"],
    data: {
        'items': [
            { "receiveid": "901", "reason": "特店代號格式錯誤", "receivecode": "931", "remark": "交易失敗" },
            { "receiveid": "902", "reason": "端末機代號格式錯誤", "receivecode": "932", "remark": "1.OrderID為EC系統交易序號對應商店指派的「交易訂單編號」。2.當商店執行HPP Plug-In時，Hosted Pay Page 會偵測該編號是否唯一(未執行過HPP Plug-In)，當偵測到同一編號重複執行Plug-In時，系統回覆「932-訂單編號重複執行Plug-In」之錯誤訊息" },
            { "receiveid": "903", "reason": "訂單編號格式錯誤", "receivecode": "933", "remark": "訂單編號不存在" },
            { "receiveid": "904", "reason": "交易金額格式錯誤", "receivecode": "934", "remark": "發卡行3D認證數字簽章驗章失敗" },
            { "receiveid": "905", "reason": "回應網址格式錯誤", "receivecode": "935", "remark": "發卡行3D認證主機回覆認證錯誤" },
            { "receiveid": "906", "reason": "請輸入分期期數", "receivecode": "936", "remark": "超過特店月限額" },
            { "receiveid": "907", "reason": "交易模式輸入錯誤!! '0':一般交易 '1':分期交易'2':紅利折抵交易", "receivecode": "937", "remark": "單筆交易金額不能超過10萬元" },
            { "receiveid": "908", "reason": "特店代號:不存在", "receivecode": "939", "remark": "尚未與該卡別簽約" },
            { "receiveid": "909", "reason": "此特店代號已解約", "receivecode": "589", "remark": "Blocked BIN" },
            { "receiveid": "910", "reason": "貴店網址與本中心註冊網址不同，請查明後再使用，謝謝", "receivecode": "998", "remark": "轉交發卡行進行3D認證" },
            { "receiveid": "911", "reason": "訂單編號重複", "receivecode": "999", "remark": "系統錯誤" },
            { "receiveid": "912", "reason": "端末機代號：不存在", "receivecode": "00、08、11", "remark": "授權交易成功" },
            { "receiveid": "913", "reason": "端末機代號已停用", "receivecode": "03、05、06、09、12-15、30-31、39、51、55-58、61-63、65、68、75-92、94、N1-N9、O0-O9、P0、P2、P3、P5、P7、P8、Q0-Q4、Q6-Q9、R0-R8、S4-S9、T5、A3、A9", "remark": "拒絕交易" },
            { "receiveid": "914", "reason": "資料庫AUTH_RECORD新增失敗", "receivecode": "33、54", "remark": "卡片過期(Expire card)" },
            { "receiveid": "915", "reason": "資料庫AUTH_RECORD讀取失敗", "receivecode": "(*)87", "remark": "卡片末三碼錯誤" },
            { "receiveid": "916", "reason": "連接授權主機失敗", "receivecode": "AY", "remark": "交易逾時(發卡行之授權訊息於法於規定之時間內(25秒)回覆至授權主機)" },
            { "receiveid": "917", "reason": "HPPRequest不支援此呼叫方法(僅支援GET/POST)", "receivecode": "AI", "remark": "紅利資料錯誤  remark:AI(發卡行回覆之紅利資料有誤)" },
            { "receiveid": "918", "reason": "交易逾時[Session 資料不一致]", "receivecode": "AK、AL", "remark": "不支援紅利交易 remark:AK(特店針對此卡別之紅利交易已過期)AL(特店針對此卡別無紅利交易之功能)" },
            { "receiveid": "919", "reason": "易逾時[Session=null]", "receivecode": "01、02、P1、P4、P6、P9、T4", "remark": "請與發卡銀行聯絡(Call Bank)" },
            { "receiveid": "920", "reason": "卡號錯誤", "receivecode": "04、07、34-38、41、43、Q5", "remark": "沒收卡面(Pickup)" },
            { "receiveid": "922", "reason": "3D交易認證錯誤", "receivecode": "T2", "remark": "交易日期錯誤" },
            { "receiveid": "923", "reason": "卡片效期格式錯誤", "receivecode": "AD、AE", "remark": "分期資料錯誤 remark:AD(分期交易之分期數不對)AE(發卡行回覆之分期資料有誤)" },
            { "receiveid": "924", "reason": "端末機不支援HPP功能", "receivecode": "AH、AG", "remark": "不支援分期交易  remark:AH(特店針對此卡別之分期交易已過期)AG(特店針對此卡別無分期交易之功能)" },
            { "receiveid": "926", "reason": "端末機尚未建檔", "receivecode": "", "remark": "" },
            { "receiveid": "930", "reason": "特店回覆Plug=In 資料檢核錯誤", "receivecode": "", "remark": "" }
          
        ]
    },
    proxy: {
        type: 'memory',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

Ext.onReady(function () {
    var CreditCardMsgGrid = Ext.create('Ext.grid.Panel', {
        id: 'CreditCardMsgGrid',
        store: Ext.data.StoreManager.lookup('CreditCardMsgStore'),
        //width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "傳回值", dataIndex: 'receiveid', width: 60, align: 'center' },
            { header: "說明", dataIndex: 'reason', width: 400, align: 'center' },
            { header: "傳回值", dataIndex: 'receivecode', width: 300, align: 'center' },
            { header: "說明", dataIndex: 'remark', width: 800, align: 'center' }           
        ],       
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
        title: "信用卡回傳信息",
        layout: 'fit',
        items: [CreditCardMsgGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //CreditCardMsgGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   
});





