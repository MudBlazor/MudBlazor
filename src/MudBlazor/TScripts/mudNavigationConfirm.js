"use strict";

// noinspection JSUnusedGlobalSymbols
/** This is the companion class for the MudBlazor.MudNavigationConfirm component. */
class MudNavigationConfirm {
    constructor() {
        this.isEnabled = false;
        this._handleBeforeUnload = this._handleBeforeUnload.bind(this);
    }

    enable() {
        if (this.isEnabled) {
            return;
        }
        this.isEnabled = true;
        
        window.addEventListener('beforeunload', this._handleBeforeUnload);
    }

    disable() {
        if (!this.isEnabled) {
            return;
        }
        this.isEnabled = false;

        window.removeEventListener('beforeunload', this._handleBeforeUnload);
    }

    handleBeforeNavigation() {
        if (!this.isEnabled) {
            return true;
        }

        const language = new Intl.Locale(navigator.language).language;
        const message = this._getLocalizedMessage(language);
        return window.confirm(message);
    }

    _handleBeforeUnload(e) {
        if (this.isEnabled) {
            e.preventDefault();
            e.returnValue = '';
            return '';
        }
    }

    _getLocalizedMessage(language) {
        const messages = {
            'en': 'Leave site? Changes you made may not be saved.',
            'en-US': 'Leave site? Changes you made may not be saved.',
            'en-GB': 'Leave site? Changes you made may not be saved.',

            'de': 'Seite verlassen? Änderungen, die Sie vorgenommen haben, werden möglicherweise nicht gespeichert.',
            'de-DE': 'Seite verlassen? Änderungen, die Sie vorgenommen haben, werden möglicherweise nicht gespeichert.',
            'de-AT': 'Seite verlassen? Änderungen, die Sie vorgenommen haben, werden möglicherweise nicht gespeichert.',

            'es': '¿Salir del sitio? Es posible que los cambios que realizaste no se guarden.',
            'fr': 'Quitter le site ? Les modifications que vous avez apportées pourraient ne pas être enregistrées.',
            'it': 'Lasciare il sito? Le modifiche apportate potrebbero non essere salvate.',

            'pt': 'Sair do site? As alterações que você fez podem não ser salvas.',
            'pt-BR': 'Sair do site? As alterações que você fez podem não ser salvas.',

            'ja': 'サイトを離れますか？行った変更が保存されない可能性があります。',
            'zh': '离开网站？您所做的更改可能不会被保存。',
            'zh-CN': '离开网站？您所做的更改可能不会被保存。',
            'zh-TW': '離開網站？您所做的變更可能不會被儲存。',

            'ru': 'Покинуть сайт? Внесённые вами изменения могут быть не сохранены.',
            'nl': 'Site verlaten? Wijzigingen die u heeft aangebracht, worden mogelijk niet opgeslagen.',
            'pl': 'Opuścić stronę? Wprowadzone zmiany mogą nie zostać zapisane.',
            'sv': 'Lämna webbplatsen? Ändringar du har gjort kanske inte sparas.',
            'no': 'Forlate nettstedet? Endringer du har gjort blir kanskje ikke lagret.',
            'da': 'Forlade siden? Ændringer, du har foretaget, gemmes muligvis ikke.',
            'fi': 'Poistutaanko sivustolta? Tekemäsi muutokset eivät välttämättä tallennu.',

            'ko': '사이트를 떠나시겠습니까? 변경한 내용이 저장되지 않을 수 있습니다.',
            'tr': 'Siteden ayrılmak istiyor musunuz? Yaptığınız değişiklikler kaydedilmeyebilir.',

            'ar': 'مغادرة الموقع؟ قد لا يتم حفظ التغييرات التي أجريتها.',
            'he': 'לעזוב את האתר? השינויים שביצעת עשויים שלא להישמר.',

            'cs': 'Opustit stránku? Provedené změny nemusí být uloženy.',
            'hu': 'Elhagyja az oldalt? Előfordulhat, hogy az elvégzett módosítások nem lesznek mentve.',
            'ro': 'Părăsiți site-ul? Este posibil ca modificările efectuate să nu fie salvate.',
            'uk': 'Залишити сайт? Внесені вами зміни можуть не зберегтися.',

            'th': 'ออกจากเว็บไซต์หรือไม่? การเปลี่ยนแปลงที่คุณทำอาจไม่ถูกบันทึก',
            'vi': 'Rời khỏi trang web? Các thay đổi bạn đã thực hiện có thể không được lưu.',
            'id': 'Tinggalkan situs? Perubahan yang Anda buat mungkin tidak tersimpan.',
            'ms': 'Tinggalkan laman? Perubahan yang anda buat mungkin tidak disimpan.'
        };

        if (messages[language]) {
            return messages[language];
        }

        const baseLanguage = language ? language.split('-')[0] : 'en';
        if (messages[baseLanguage]) {
            return messages[baseLanguage];
        }

        return messages['en'];
    }
}

if (!window.mudNavigationConfirm) {
    window.mudNavigationConfirm = new MudNavigationConfirm();
}