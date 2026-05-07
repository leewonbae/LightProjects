<template>
  <div class="page-result">
    <div class="panel">
      <h3 class="panel-heading">조회 결과</h3>

      <div class="panel-body">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "PageResult",
};
</script>
