from PIL import Image, ImageDraw

"""
失去分路纹理生成脚本
独立运行生成失去分路纹理
"""

def create_lost_diversion_texture():
    """创建失去分路纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 橘红色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FF7F50")
    
    return img

def main():
    """主函数"""
    print("生成失去分路纹理...")
    img = create_lost_diversion_texture()
    img.save("lost_diversion.png")
    print("失去分路纹理已生成: lost_diversion.png")

if __name__ == "__main__":
    main()