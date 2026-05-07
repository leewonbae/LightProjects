<template>
  <div class="listpage-wrapper">
    <div class="listpage">
      <label for="pageSize"><span>페이지 크기</span></label>
      <select v-model="pageValue" name="pageSize" class="list-page-size">
        <option value="10" selected="selected">10</option>
        <option value="20">20</option>
        <option value="30">30</option>
        <option value="50">50</option>
        <option value="100">100</option>
      </select>
    </div>
  </div>
</template>
<script>
export default {
  name: "PageSize",
  props: { pageSize: Number },
  computed: {
    pageValue: {
      get() {
        return this.pageSize;
      },
      set(value) {
        this.$emit("update:pageSize", parseInt(value));
      },
    },
  },
};
</script>
