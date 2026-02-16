from PIL import Image, ImageDraw

"""
双黄纹理生成脚本
独立运行生成双黄纹理
"""

def create_double_yellow_texture():
    """创建双黄纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 上半部分黄色填充
    draw.rectangle([0, 0, 16, 8], fill="#FFFF00")
    
    # 下半部分黄色填充
    draw.rectangle([0, 9, 16, 16], fill="#FFFF00")
    
    # 中间黑色分隔线
    draw.line([(0, 8), (16, 8)], fill="#000000", width=1)
    
    return img

def main():
    """主函数"""
    print("生成双黄纹理...")
    img = create_double_yellow_texture()
    img.save("double_yellow.png")
    print("双黄纹理已生成: double_yellow.png")

if __name__ == "__main__":
    main()