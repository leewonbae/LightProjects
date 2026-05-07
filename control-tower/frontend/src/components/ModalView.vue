<template>
  <div v-show="openModalView" :class="{ modal: 'modal', 'open-modal': true }">
    <span class="modal-background" @click="closeModal"></span>

    <div class="modal-contents">
      <div class="btn-close-wrapper">
        <span class="btn-closes" @click="closeModal">×</span>
      </div>
      <div class="modals-header">{{ title }}</div>

      <slot></slot>

      <div class="modals-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ModalView",
  props: {
    openModal: Boolean,
    title: String,
  },
  emits: ["close-modal"],
  computed: {
    openModalView: {
      get() {
        return this.openModal;
      },
      set(value) {
        if (!value) {
          this.$emit("close-modal");
        }
      },
    },
  },
  methods: {
    closeModal() {
      this.openModalView = false;
    },
  },
};
</script>
